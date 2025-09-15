using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Modules;
using JetBrains.Annotations;
using MEC;
using YamlDotNet.Serialization;
using Firearm = Exiled.API.Features.Items.Firearm;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunLogicer)]
    public class GrenadeLauncher : CustomWeapon
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GunLogicer;
        public override uint Id { get; set; } = 46;
        public override string Name { get; set; } = "<color=#FF0000>ADATS</color>";
        public override string Description { get; set; } = "A grenade launcher";
        public override float Weight { get; set; } = 3;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new ()
                {
                    Chance = 40,
                    Location = SpawnLocationType.Inside079Armory,
                },
            }
        };
        public override byte ClipSize { get; set; } = 1;
        [Description("Set to false if you want to include Custom Grenades (such as Cluster Grenades) in the grenade launcher. NOTE: This will not make the grenade launcher launch a Cluster Grenade")]
        public bool IgnoreCustomGrenades { get; set; } = true;

        //public float GrenadeFuseTime { get; set; } = 1.5f;
        public bool UseGrenadesToReload { get; set; } = true;

        [Description(
            "If UseGrenadesToReload is true, this message will be shown to the player to be told to dry fire it")]
        public string ReloadMessageDryfire { get; set; } = "You need a grenade, and to dry fire ADATS to reload it";
        
        public string FullForceSetMessage { get; set; } = "The ADATS is set to full force mode";
        public string HalfForceSetMessage { get; set; } = "The ADATS is set to half force mode";
        public string LaunchTypeImpactSetMessage { get; set; } = "The ADATS is set to impact detonation mode";
        public string LaunchTypeRollerSetMessage { get; set; } = "The ADATS is set to roller detonation mode";
        [Description("If true, the player will get a hint when a message is displayed, otherwise it will be a broadcast")]
        public bool UseHints { get; set; } = true;
        [Description("How long the hint/broadcast will be shown for")]
        public float MessageDuration { get; set; } = 3f;
        [Description("Sometimes you're able to get more than what ClipSize is set to when reloading, if this is set to true, it will check and correct the ammo count")]
        public bool FixOverClipSizeBug { get; set; } = true;
        private ProjectileType GrenadeType { get; set; } = ProjectileType.FragGrenade;
        [CanBeNull] 
        private CustomGrenade _loadedCustomGrenade;
        [YamlIgnore]
        private Dictionary<Player, bool> FullForceMode { get; set; } = new();
        [YamlIgnore]
        private Dictionary<Player, bool> LaunchTypeMode { get; set; } = new();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.DryfiringWeapon += OnDryfiringWeapon;
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.DryfiringWeapon += OnDryfiringWeapon;
            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            FullForceMode.Clear();
            LaunchTypeMode.Clear();
            base.OnWaitingForPlayers();
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (!FullForceMode.ContainsKey(player))
                FullForceMode.Add(player, true);
            if (!LaunchTypeMode.ContainsKey(player))
                LaunchTypeMode.Add(player, true);
            base.OnAcquired(player, item, displayMessage);
        }
        public void ToggleForceMode(Player player)
        {
            if (!FullForceMode.ContainsKey(player))
                FullForceMode.Add(player, true);
            
            FullForceMode[player] = !FullForceMode[player];
            Log.Debug($"VVUP Custom Items: Grenade Launcher: {player.Nickname} toggled force mode to {(FullForceMode[player]? "Full Force" : "Half Force")}");
            if (UseHints)
                player.ShowHint(FullForceMode[player]? FullForceSetMessage : HalfForceSetMessage, MessageDuration);
            else
                player.Broadcast((ushort)MessageDuration, FullForceMode[player]? FullForceSetMessage : HalfForceSetMessage);
        }
        public void ToggleLaunchTypeMode(Player player)
        {
            if (!LaunchTypeMode.ContainsKey(player))
                LaunchTypeMode.Add(player, true);
            
            LaunchTypeMode[player] = !LaunchTypeMode[player];
            Log.Debug($"VVUP Custom Items: Grenade Launcher: {player.Nickname} toggled launch type mode to {(LaunchTypeMode[player]? "Impact" : "Roller")}");
            if (UseHints)
                player.ShowHint(LaunchTypeMode[player]? LaunchTypeImpactSetMessage : LaunchTypeRollerSetMessage, MessageDuration);
            else
                player.Broadcast((ushort)MessageDuration, LaunchTypeMode[player]? LaunchTypeImpactSetMessage : LaunchTypeRollerSetMessage);
        }
        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.IsAllowed = false;

            if (ev.Player.CurrentItem is Firearm firearm)
            {
                if (firearm.MagazineAmmo > ClipSize && FixOverClipSizeBug)
                {
                    Log.Debug("VVUP Custom Items: Grenade Launcher Impact: Fixing ammo count due to over clip size bug");
                    firearm.MagazineAmmo = ClipSize;
                }
                firearm.MagazineAmmo -= 1;
            }
            Log.Debug($"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} fired, firing a {GrenadeType}, Is Full Force: {FullForceMode[ev.Player]}, Is Impact: {LaunchTypeMode[ev.Player]}");
            Projectile projectile = GrenadeType switch
            {
                ProjectileType.FragGrenade => ev.Player.ThrowGrenade(GrenadeType, FullForceMode[ev.Player]).Projectile,
                ProjectileType.Flashbang => ev.Player.ThrowGrenade(GrenadeType, FullForceMode[ev.Player]).Projectile,
                //ProjectileType.Scp018 => ev.Player.ThrowGrenade(GrenadeType, FullForceMode[ev.Player]).Projectile,
                ProjectileType.Scp2176 => ev.Player.ThrowGrenade(GrenadeType, FullForceMode[ev.Player]).Projectile,
                ProjectileType.Coal => ev.Player.ThrowGrenade(GrenadeType, FullForceMode[ev.Player]).Projectile,
                ProjectileType.SpecialCoal => ev.Player.ThrowGrenade(GrenadeType, FullForceMode[ev.Player]).Projectile,
                ProjectileType.Snowball => ev.Player.ThrowGrenade(GrenadeType, FullForceMode[ev.Player]).Projectile,
            };
            
            if (LaunchTypeMode[ev.Player])
                projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, projectile.Base);
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (UseGrenadesToReload)
            {
                if (UseHints)
                    ev.Player.ShowHint(ReloadMessageDryfire, MessageDuration);
                else
                    ev.Player.Broadcast((ushort)MessageDuration, ReloadMessageDryfire);
                return;
            }

            Log.Debug($"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} reloaded the Grenade Launcher Impact with regular ammo.");
        }

        private void OnDryfiringWeapon(DryfiringWeaponEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem) && ev.Player.CurrentItem is Firearm { MagazineAmmo: 0 } firearm && UseGrenadesToReload)
            {
                Log.Debug(
                    $"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} is reloading the Grenade Launcher Impact with grenades.");
                foreach (Item item in ev.Player.Items.ToList())
                {
                    Log.Debug($"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} has {item.Type}");
                    if (item.Type != ItemType.GrenadeHE && item.Type != ItemType.GrenadeFlash &&
                        /*item.Type != ItemType.SCP018 &&*/ item.Type != ItemType.SCP2176
                        && item.Type != ItemType.Coal && item.Type != ItemType.SpecialCoal && item.Type != ItemType.Snowball)
                    {
                        Log.Debug(
                            $"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} has a {item.Type}, not a grenade, skipping.");
                        continue;
                    }

                    if (TryGet(item, out CustomItem? customItem))
                    {
                        if (IgnoreCustomGrenades)
                        {
                            Log.Debug(
                                $"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} has a {item.Type}, but it's a custom grenade, skipping.");
                            continue;
                        }

                        if (customItem is CustomGrenade customGrenade)
                        {
                            _loadedCustomGrenade = customGrenade;
                            Log.Debug(
                                $"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} has a {item.Type}, it's a custom grenade, setting it to {_loadedCustomGrenade.Name}");
                        }
                    }

                    ev.Player.DisableEffect(EffectType.Invisible);
                    GrenadeType = item.Type switch
                    {
                        ItemType.GrenadeHE => ProjectileType.FragGrenade,
                        ItemType.GrenadeFlash => ProjectileType.Flashbang,
                        //ItemType.SCP018 => ProjectileType.Scp018,
                        ItemType.SCP2176 => ProjectileType.Scp2176,
                        ItemType.Coal => ProjectileType.Coal,
                        ItemType.SpecialCoal => ProjectileType.SpecialCoal,
                        ItemType.Snowball => ProjectileType.Snowball
                    };
                    ev.Player.RemoveItem(item);
                    ushort ammo762Amount = ev.Player.GetAmmo(AmmoType.Nato762);
                    ev.Player.AddAmmo(AmmoType.Nato762, 1);
                    Timing.CallDelayed(0.5f, () =>
                    {
                        if (firearm.Base.TryGetModule(out AnimatorReloaderModuleBase reloaderModule))
                        {
                            //I dont know which one works, but it does so Im keeping it.
                            reloaderModule.ClientTryReload();
                            reloaderModule.ServerTryReload();
                        }
                        Log.Debug(
                            $"VVUP Custom Items: Grenade Launcher Impact: Server-side reload triggered for {ev.Player.Nickname}");
                    });
                    Timing.CallDelayed(4f, () =>
                    {
                        firearm.MagazineAmmo = ClipSize;
                        ev.Player.SetAmmo(AmmoType.Nato762, ammo762Amount);
                    });
                    Log.Debug(
                        $"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} reloaded the Grenade Launcher Impact with a {GrenadeType} grenade.");
                    return;
                }
            }
        }
        protected override void OnReloaded(ReloadedWeaponEventArgs ev)
        {
            Log.Debug($"VVUP Custom Items: Grenade Launcher Impact: {ev.Player.Nickname} reloaded the Grenade Launcher Impact setting Magazine Ammo to {ClipSize}.");
            ev.Firearm.MagazineAmmo = ClipSize;
        }
    }
}