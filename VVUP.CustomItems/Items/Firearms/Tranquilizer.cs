using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pools;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using Item = Exiled.API.Features.Items.Item;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Warhead = Exiled.API.Features.Warhead;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunCOM18)]
    public class Tranquilizer : CustomWeapon, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GunCOM18;
        public override uint Id { get; set; } = 28;
        public override string Name { get; set; } = "<color=#0096FF>Silent Serenade</color>";

        public override string Description { get; set; } =
            "A modified COM-18, firing tranquilizer darts. Firing it against the same target will build a resistance to it";

        public override float Weight { get; set; } = 1.55f;
        public override float Damage { get; set; }
        public override byte ClipSize { get; set; } = 2;
        
        [Description("Determines the likelyhood that SCPs will resist the tranquilizer")]
        public int ScpResistChance { get; set; } = 75;

        [Description("After a player gets tranquilized, how much resistance should the player get from future transquilizations")]
        public float ResistanceModifer { get; set; } = 1.75f;

        [Description("Should tranquilized players drop items?")]
        public bool DropItems { get; set; } = true;

        [Description("The base duration for players to be tranquilized for")]
        public float Duration { get; set; } = 5f;

        [Description("Determines if 096 rage ends if 096 gets tranquilized")]
        public bool End096Rage { get; set; } = true;

        [Description("Can players be Tranquilized in elevators?")]
        public bool TranquilizeInElevators { get; set; } = false;
        
        [Description("What's the tranquilized reason")]
        public string TranquilizedReason { get; set; } = "Tranquilized";

        [Description("What effects should be applied when a player gets tranquilized?")]
        public List<EffectType> Effects { get; set; } = new List<EffectType>
        {
            EffectType.Invisible,
            EffectType.AmnesiaItems,
            EffectType.AmnesiaVision,
            EffectType.Ensnared,
            EffectType.Flashed,
        };

        private Dictionary<PlayerAPI, float> _tranquilizedPlayers = new();
        private List<PlayerAPI> _activeTranquilizedPlayers = new();
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.InsideGr18,
                },
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.Inside173Armory,
                },
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.InsideLczCafe,
                },
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 20,
                    Offset = new Vector3(-3.25f, 1, -0.78f),
                    Room = RoomType.Hcz096,
                },
            },
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 150, 255, 127);
        public float GlowRange { get; set; } = 0.25f;

        protected override void SubscribeEvents()
        {
            PlayerEvent.PickingUpItem += OnDeniableEvent;
            PlayerEvent.ChangingItem += OnDeniableEvent;
            PlayerEvent.VoiceChatting += OnDeniableEvent;
            //PlayerEvent.ReloadingWeapon += OnReloading;
            Scp049.StartingRecall += OnDeniableEvent;
            Scp106.Teleporting += OnDeniableEvent;
            Scp096.Charging += OnDeniableEvent;
            Scp096.Enraging += OnDeniableEvent;
            Scp096.AddingTarget += OnDeniableEvent;
            Scp173.Blinking += OnDeniableEvent;
            Scp173.BlinkingRequest += OnDeniableEvent;
            Scp939.PlacingAmnesticCloud += OnDeniableEvent;
            base.SubscribeEvents();
        }
        
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.PickingUpItem -= OnDeniableEvent;
            PlayerEvent.ChangingItem -= OnDeniableEvent;
            PlayerEvent.VoiceChatting -= OnDeniableEvent;
            //PlayerEvent.ReloadingWeapon -= OnReloading;
            Scp049.StartingRecall -= OnDeniableEvent;
            Scp106.Teleporting -= OnDeniableEvent;
            Scp096.Charging -= OnDeniableEvent;
            Scp096.Enraging -= OnDeniableEvent;
            Scp096.AddingTarget -= OnDeniableEvent;
            Scp173.Blinking -= OnDeniableEvent;
            Scp173.BlinkingRequest -= OnDeniableEvent;
            Scp939.PlacingAmnesticCloud -= OnDeniableEvent;
            base.UnsubscribeEvents();
        }
        /*private void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Timing.CallDelayed(2f, () =>
            {
                Log.Debug($"VVUP Custom Items: Tranquilizer, {ev.Player.Nickname} has started reloading, setting correct ammo");
                ev.Firearm.MagazineAmmo = ClipSize;
            });
        }*/
        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == ev.Player)
                return;

            ev.Amount = Damage;

            if (ev.Player.Role.Team == Team.SCPs)
            {
                int randomNumber = Base.GetRandomNumber.GetRandomInt(1, 101);
                Log.Debug(
                    $"VVUP Custom Items: Tranquilizer, rolled {randomNumber}, checking if SCP can be tranquilized");
                if (randomNumber <= ScpResistChance)
                {
                    Log.Debug(
                        $"VVUP Custom Items: Tranquilizer, {randomNumber} is too low for the effect, resist chance for SCPs is {ScpResistChance}");
                    return;
                }
            }

            if (!TranquilizeInElevators && ev.Player.Lift != null)
            {
                Log.Debug("VVUP Custom Items: Tranquilizer, Tranquilize players in elevators is false, ending execution (anti void protection)");
                return;
            }

            float duration = Duration;
                
            if(!_tranquilizedPlayers.TryGetValue(ev.Player, out _))
                _tranquilizedPlayers.Add(ev.Player, 1);

            _tranquilizedPlayers[ev.Player] *= ResistanceModifer;
            Log.Debug($"VVUP Custom Items: Tranquilizer, Resistance duration modifer is {_tranquilizedPlayers[ev.Player]} for {ev.Player.Nickname}");

            duration -= _tranquilizedPlayers[ev.Player];
            Log.Debug($"VVUP Custom Items: Tranquilizer, Tranquilize time is {duration} for {ev.Player.Nickname}");
                
            if (duration > 0)
                Timing.RunCoroutine(DoTranq(ev.Player, duration));
        }

        private IEnumerator<float> DoTranq(PlayerAPI player, float duration)
        {
            _activeTranquilizedPlayers.Add(player);
            Vector3 oldPos = player.Position;
            Item previousItem = player.CurrentItem;
            Vector3 previousScale = player.Scale;
            float newHealth = player.Health - Damage;
            List<StatusEffectBase> activeEffects = ListPool<StatusEffectBase>.Pool.Get();
            player.CurrentItem = null;
            Log.Debug($"VVUP Custom Items: Tranquilizer, storing {player.Nickname}'s previous stats");
            if (newHealth <= 0)
                yield break;
            
            activeEffects.AddRange(player.ActiveEffects.Where(effect => effect.IsEnabled));
            Log.Debug($"VVUP Custom Items: Tranquilizer, saving {player.Nickname}'s previous effects");
            try
            {
                if (DropItems)
                {
                    Log.Debug($"VVUP Custom Items: Tranquilizer, Drop Items are true, checking {player.Nickname}'s inventory for items");
                    if (player.Items.Count > 0)
                    {
                        foreach (Item item in player.Items.ToList())
                        {
                            if (TryGet(item, out CustomItem? customItem))
                            {
                                customItem?.Spawn(player.Position, item, player);
                                player.RemoveItem(item);
                            }
                        }
                        Log.Debug($"VVUP Custom Items: Tranquilizer, Dropping {player.Nickname}'s inventory");
                        player.DropItems();
                    }
                }
                player.CurrentItem = null;
            }
            catch (Exception ex)
            {
                Log.Error($"VVUP Custom Items: Tranquilizer, {nameof(DoTranq)}: {ex}");
            }

            Ragdoll? ragdoll = null;

            if (player.Role != RoleTypeId.Scp106)
            {
                ragdoll = Ragdoll.CreateAndSpawn(player.Role, player.DisplayNickname, TranquilizedReason,
                    player.Position, player.ReferenceHub.PlayerCameraReference.rotation, player);
                Log.Debug($"VVUP Custom Items: Tranquilizer, spawning {player.Role} ragdoll at {player.Nickname}'s location");
            }

            if (player.Role is Scp096Role scp && End096Rage)
            {
                scp.RageManager.ServerEndEnrage();
                Log.Debug($"VVUP Custom Items: Tranquilizer, End096Rage is true and {player.Nickname} is SCP-096, ending rage");
            }

            try
            {
                foreach (EffectType effect in Effects)
                {
                    Log.Debug($"VVUP Custom Items: Tranquilizer, applying {effect} to {player.Nickname}");
                    player.EnableEffect(effect, duration);
                }
                player.Scale = Vector3.one * 0.2f;
                player.Health = newHealth;
                player.IsGodModeEnabled = true;
            }
            catch (Exception ex)
            {
                Log.Error($"VVUP Custom Items: Tranquilizer, {ex}");
            }

            Log.Debug($"VVUP Custom Items: Tranquilizer, waiting for {duration} seconds");
            
            yield return Timing.WaitForSeconds(duration);

            try
            {
                if (ragdoll != null)
                {
                    Log.Debug($"VVUP Custom Items: Tranquilizer, removing {ragdoll}");
                    NetworkServer.Destroy(ragdoll.GameObject);
                }

                if (player.GameObject == null)
                {
                    Log.Debug("VVUP Custom Items: Tranquilizer, GameObject is null, ending tranq execution");
                    yield break;
                }

                Log.Debug($"VVUP Custom Items: Tranquilizer, Restoring {player.Nickname}'s previous stats");
                newHealth = player.Health;
                player.IsGodModeEnabled = false;
                player.Scale = previousScale;
                player.Health = newHealth;

                if (!DropItems)
                {
                    Log.Debug($"VVUP Custom Items: Tranquilizer, Since Drop Items was false, restoring {player.Nickname}'s previous item");
                    player.CurrentItem = previousItem;
                }

                foreach (StatusEffectBase effect in activeEffects.Where(effect => (effect.Duration - duration) > 0))
                {
                    Log.Debug($"VVUP Custom Items: Tranquilizer, restoring {player.Nickname}'s {effect} which has {effect.Duration} seconds remaining");
                    player.EnableEffect(effect, effect.Duration);
                }

                _activeTranquilizedPlayers.Remove(player);
                ListPool<StatusEffectBase>.Pool.Return(activeEffects);
            }
            catch (Exception ex)
            {
                Log.Error($"VVUP Custom Items: Tranquilizer, {nameof(DoTranq)}: {ex}");
            }

            if (Warhead.IsDetonated && player.Zone != ZoneType.Surface)
            {
                Log.Debug($"VVUP Custom Items: Tranquilizer, {player.Nickname}'s is in the facility when nuke went off, killing them");
                player.Kill(DamageType.Warhead);
            }
            
            player.Position = oldPos;
        }

        private void OnDeniableEvent(IDeniableEvent ev)
        {
            if (ev is IPlayerEvent evPlayerEvent && _activeTranquilizedPlayers.Contains(evPlayerEvent.Player))
                ev.IsAllowed = false;
        }
    }
}