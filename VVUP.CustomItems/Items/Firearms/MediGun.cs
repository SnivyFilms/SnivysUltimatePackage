using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using PlayerRoles;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunFSP9)]
    public class MediGun : CustomWeapon
    {
        public override ItemType Type { get; set; } = ItemType.GunFSP9;
        
        public override uint Id { get; set; } = 27;
        public override string Name { get; set; } = "<color=#0096FF>Phantom Pulse</color>";

        public override string Description { get; set; } =
            "A specialized weapon which applies healing to friendlies and revives zombies into humans";

        public override float Weight { get; set; } = 1.5f;
        
        public override float Damage { get; set; } = 0;

        public override byte ClipSize { get; set; } = 10;

        [Description("Does the medigun friendly fire? (Only an option for FF on servers)")]
        public override bool FriendlyFire { get; set; } = false;
        
        public bool HealZombies { get; set; } = true;

        public float AhpRequiredForZombieHeal { get; set; } = 200f;
        [Description("Determines if Serpents Hand can revive zombies to their side")]
        public bool ZombieHealingBySerpents { get; set; } = false;

        [Description("What is the CustomRole ID for Serpents Hand to set the revive zombie to")]
        public uint SerpentsHandCustomRoleId { get; set; } = 27;
        [Description("What is the healing modifier? (Example: 1 = Damage * 1, so a bullet that does 5 damage will heal 5 health instead.")]
        public float HealingModifer { get; set; } = 1f;
        [Description("Determines how much AHP human players can get")]
        public float MaxAhpAmount { get; set; } = 30f;
        [Description("Deterimines if AHP drains")]
        public bool AhpDecay { get; set; } = false;

        [Description("Should revived players be given a loadout?")]
        public bool GrantLoadoutOnRevive { get; set; } = false;
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGr18,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGateA,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGateB,
                },
            }
        };

        protected override void OnShot(ShotEventArgs ev)
        {
            if (ev.Target == null)
                return;
            if (ev.Player == ev.Target)
                return;
            if (ev.Player.Role.Team == ev.Target.Role.Team)
            {
                float amount;
                if (Server.FriendlyFire)
                    amount = (ev.Damage * 2) * HealingModifer;
                else
                    amount = ev.Damage * HealingModifer;
                ev.Target.Heal(amount);
                Log.Debug($"VVUP Custom Items: Medigun healing {ev.Target.Nickname} for {amount}");
                if (ev.Target.Health >= ev.Target.MaxHealth && ev.Target.ArtificialHealth < MaxAhpAmount)
                {
                    float decay = !AhpDecay ? 0f : 1.2f;
                    ev.Target.AddAhp(amount, MaxAhpAmount, decay);
                    Log.Debug($"VVUP Custom Items: Medigun adding {amount} AHP to {ev.Target.Nickname}");
                }
                ev.Player.ShowHitMarker();
            }
            else if (ev.Target.Role == RoleTypeId.Scp0492 && HealZombies)
            {
                if (!ev.Target.ActiveArtificialHealthProcesses.Any())
                    ev.Target.AddAhp(0, AhpRequiredForZombieHeal, persistant: true);
                ev.Target.ArtificialHealth += ev.Damage;
                if (ev.Target.ArtificialHealth >= AhpRequiredForZombieHeal)
                {
                    switch (ev.Player.Role.Side)
                    {
                        case Side.Mtf:
                            ev.Target.Role.Set(RoleTypeId.NtfPrivate, SpawnReason.ForceClass,
                                GrantLoadoutOnRevive ? RoleSpawnFlags.AssignInventory : RoleSpawnFlags.None);
                            break;
                        case Side.ChaosInsurgency:
                            ev.Target.Role.Set(RoleTypeId.ChaosConscript, SpawnReason.ForceClass,
                                GrantLoadoutOnRevive ? RoleSpawnFlags.AssignInventory : RoleSpawnFlags.None);
                            break;
                        case Side.Tutorial when ZombieHealingBySerpents:
                            CustomRole.Get(SerpentsHandCustomRoleId)?.AddRole(ev.Target);
                            break;
                    }
                }

                ev.CanHurt = false;
                ev.Player.ShowHitMarker();
            }
        }
    }
}