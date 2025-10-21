using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;
using VVUP.Base;
using VVUP.CustomItems.API;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.Lantern)]
    public class PhantomLantern : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.Lantern;
        public override uint Id { get; set; } = 24;
        public override string Name { get; set; } = "<color=#0096FF>Phantom Lantern</color>";
        public override string Description { get; set; } = "'Limbo is no place for a soul like yours'";
        public override float Weight { get; set; } = 0.5f;
        public float EffectDuration { get; set; } = 150f;
        private List<PlayerAPI> _playersWithEffect = new List<PlayerAPI>();
        private CoroutineHandle phantomLanternCoroutine;
        public List<ApplyEffects> Effects = new List<ApplyEffects>()
        {
            new()
            {
                EffectType = EffectType.Ghostly,
                Duration = 150,
                AddDurationIfActive = false
            },
            new()
            {
                EffectType = EffectType.Invisible,
                Duration = 150,
                AddDurationIfActive = false
            },
            new()
            {
                EffectType = EffectType.FogControl,
                Intensity = 5,
                Duration = 150,
                AddDurationIfActive = false
            },
            new()
            {
                EffectType = EffectType.Slowness,
                Intensity = 50,
                Duration = 150,
                AddDurationIfActive = false
            },
            new()
            {
                EffectType = EffectType.AmnesiaItems,
                Duration = 150,
                AddDurationIfActive = false
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside096,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideGr18,
                },
            },
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Role = RoleTypeId.Scp106
                }
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Room = RoomType.HczTestRoom,
                    Offset = new Vector3(0.885f, 0.749f, -4.874f)
                }
            }
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 150, 255, 191);

        protected override void SubscribeEvents()
        {
            PlayerEvent.TogglingFlashlight += UsingFlashlight;
            PlayerEvent.InteractingDoor += OnInteractingDoor;
            PlayerEvent.InteractingElevator += OnInteractingElevator;
            PlayerEvent.InteractingLocker += OnInteractingLocker;
            PlayerEvent.Interacted += OnInteracted;
            PlayerEvent.Died += OnDied;
            PlayerEvent.Left += OnDisconnect;
            PlayerEvent.ChangingItem += OnChangingItem;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.TogglingFlashlight -= UsingFlashlight;
            PlayerEvent.InteractingDoor -= OnInteractingDoor;
            PlayerEvent.InteractingElevator -= OnInteractingElevator;
            PlayerEvent.InteractingLocker -= OnInteractingLocker;
            PlayerEvent.Interacted -= OnInteracted;
            PlayerEvent.Died -= OnDied;
            PlayerEvent.Left -= OnDisconnect;
            PlayerEvent.ChangingItem -= OnChangingItem;
            base.UnsubscribeEvents();
        }

        private void UsingFlashlight(TogglingFlashlightEventArgs ev)
        {
            if (_playersWithEffect.Contains(ev.Player) && Check(ev.Player.CurrentItem))
            {
                Log.Debug("VVUP Custom Items: Phantom Lantern, Disabling Effects");
                EndOfEffect(ev.Player);
            }
                
            if (!Check(ev.Player.CurrentItem))
                return;
            Log.Debug("VVUP Custom Items: Activating Phantom Lantern Effects");
            _playersWithEffect.Add(ev.Player);
            foreach (var effect in Effects)
            {
                ev.Player.DisableEffect(effect.EffectType);
                ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
            }
            phantomLanternCoroutine = Timing.RunCoroutine(PhantomLanternCoroutine(ev.Player));
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem)) 
                return;
            if (!_playersWithEffect.Contains(ev.Player))
                return;
            Timing.CallDelayed(.5f, () =>
            {
                foreach (var effect in Effects)
                {
                    ev.Player.DisableEffect(effect.EffectType);
                    ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
                }
            });
        }

        private void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (!_playersWithEffect.Contains(ev.Player))
                return;
            Timing.KillCoroutines(phantomLanternCoroutine);
            EndOfEffect(ev.Player);
        }

        private void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!_playersWithEffect.Contains(ev.Player))
                return;
            Timing.KillCoroutines(phantomLanternCoroutine);
            EndOfEffect(ev.Player);
        }
        private void OnInteracted(InteractedEventArgs ev)
        {
            if (!_playersWithEffect.Contains(ev.Player))
                return;
            Timing.KillCoroutines(phantomLanternCoroutine);
            EndOfEffect(ev.Player);
        }
        private void OnDied(DiedEventArgs ev)
        {
            // Check if the player is not null and has an active lantern effect
            if (ev.Player != null && _playersWithEffect.Contains(ev.Player))
            {
                Timing.KillCoroutines(phantomLanternCoroutine);
                EndOfEffect(ev.Player);
            }
        }

        private void OnDisconnect(LeftEventArgs ev)
        {
            // Check if the player is not null and has an active lantern effect
            if (ev.Player != null && _playersWithEffect.Contains(ev.Player))
            {
                Timing.KillCoroutines(phantomLanternCoroutine);
                _playersWithEffect.Remove(ev.Player);
            }
        }

        protected override void OnWaitingForPlayers()
        {
            Timing.KillCoroutines(phantomLanternCoroutine);
            _playersWithEffect.Clear();
        }
        public IEnumerator<float> PhantomLanternCoroutine(PlayerAPI player)
        {
            float durationRemaining = EffectDuration;
            while (durationRemaining > 0 && _playersWithEffect.Contains(player))
            {
                player.Stamina = 0;
                durationRemaining -= .5f;
                yield return Timing.WaitForSeconds(.5f);
            }
            EndOfEffect(player);
        }
        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.Player != null && _playersWithEffect.Contains(ev.Player))
                ev.IsAllowed = false;
        }
        public void EndOfEffect(PlayerAPI player)
        {
            if (player == null) 
                return;
            Log.Debug("VVUP Custom Items: Ending Phantom Lantern's Effects");
            foreach (var effect in Effects)
            {
                player.DisableEffect(effect.EffectType);
            }
            player.CurrentItem?.Destroy();
            if (_playersWithEffect.Contains(player))
                _playersWithEffect.Remove(player);
        }
    }
}