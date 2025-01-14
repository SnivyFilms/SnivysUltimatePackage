﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using MEC;
using PlayerRoles;
using UnityEngine;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace SnivysUltimatePackage.Custom.Items.Other
{
    [CustomItem(ItemType.Lantern)]
    public class PhantomLantern : CustomItem
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
        [CanBeNull]
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
            Server.WaitingForPlayers += OnWaitingForPlayers;
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
            Server.WaitingForPlayers -= OnWaitingForPlayers;
            base.UnsubscribeEvents();
        }

        private void UsingFlashlight(TogglingFlashlightEventArgs ev)
        {
            if (_playersWithEffect.Contains(ev.Player))
                return;
            if (!Check(ev.Player.CurrentItem))
                return;
            Log.Debug("VVUP Custom Items: Activating Phantom Lantern Effects");
            _playersWithEffect.Add(ev.Player);
            ev.Player.EnableEffect(EffectType.Ghostly, EffectDuration);
            ev.Player.EnableEffect(EffectType.Invisible, EffectDuration);
            ev.Player.EnableEffect(EffectType.FogControl, 5, EffectDuration);
            ev.Player.EnableEffect(EffectType.Slowness, 50, EffectDuration);
            ev.Player.EnableEffect(EffectType.AmnesiaItems, EffectDuration);
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
                ev.Player.EnableEffect(EffectType.Invisible);
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

        public void OnWaitingForPlayers()
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
            player.DisableEffect(EffectType.Ghostly);
            player.DisableEffect(EffectType.Invisible);
            player.DisableEffect(EffectType.Slowness);
            player.DisableEffect(EffectType.FogControl);
            player.DisableEffect(EffectType.AmnesiaItems);
            player.CurrentItem?.Destroy();
            if (_playersWithEffect.Contains(player))
                _playersWithEffect.Remove(player);
        }
    }
}