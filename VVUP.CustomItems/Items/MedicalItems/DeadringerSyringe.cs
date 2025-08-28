using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.MedicalItems
{
    [CustomItem(ItemType.Adrenaline)]
    public class DeadringerSyringe : CustomItem
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.Adrenaline;
        public override uint Id { get; set; } = 23;
        public override string Name { get; set; } = "Phantom Decoy Device";
        public override string Description { get; set; } = "When injected. You become light headed, which will eventually cause other effects";
        public override float Weight { get; set; } = 1.15f;
        public String OnUseMessage { get; set; } = "You become incredibly light headed";
        public float OnUseMessageTimeDuration { get; set; } = 5f;
        public String RagdollDeathReason { get; set; } = "Totally A Intentional Fatal Injection";
        public bool UsableAfterNuke { get; set; } = false;
        public bool TeleportToLightAfterDecom { get; set; } = false;
        public bool UseHints { get; set; } = true;
        [CanBeNull]
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new()
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside939Cryo
                },
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Room = RoomType.HczTestRoom,
                    Offset = new Vector3(0.885f, 0.749f, -4.874f)
                },
            },
            LockerSpawnPoints = new()
            {
                new LockerSpawnPoint()
                {
                    Chance = 25,
                    Type = LockerType.Misc,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
            },
        };
        public List<RoomType> ExcludedRooms { get; set; } = new List<RoomType>()
        {
            RoomType.EzShelter,
            RoomType.Lcz173,
            RoomType.Hcz049,
            RoomType.HczNuke,
            RoomType.EzCollapsedTunnel
        };
        protected override void SubscribeEvents()
        {
            Player.UsingItem += OnUsingItem;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItem -= OnUsingItem;
            base.UnsubscribeEvents();
        }
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            if (!UsableAfterNuke && Warhead.IsDetonated)
                return;
            Log.Debug("VVUP Custom Items: Deadringer Syring, Running methods");
            if (UseHints)
                ev.Player.ShowHint(OnUseMessage, OnUseMessageTimeDuration);
            else
                ev.Player.Broadcast((ushort)OnUseMessageTimeDuration, OnUseMessage);            
            ev.Player.EnableEffect(EffectType.Blinded, 15f, true);
            Timing.CallDelayed(3, () =>
            {
                ev.Player.EnableEffect(EffectType.Flashed, 5f, true);
                ev.Player.EnableEffect(EffectType.Invisible, 5f, true);
                ev.Player.EnableEffect(EffectType.Ensnared, 5f, true);
                ev.Player.EnableEffect(EffectType.Disabled, 60f, true);
                ev.Player.EnableEffect(EffectType.Exhausted, 15f, true);
                ev.Player.EnableEffect(EffectType.AmnesiaItems, 30f, true);
                ev.Player.EnableEffect(EffectType.AmnesiaVision, 30f, true);
                Ragdoll ragdoll = Ragdoll.CreateAndSpawn(ev.Player.Role, ev.Player.Nickname, RagdollDeathReason, ev.Player.Position, ev.Player.ReferenceHub.PlayerCameraReference.rotation);
                List<Room> rooms = Room.List.Where(room => !ExcludedRooms.Contains(room.Type)).ToList();
                if (rooms.Count > 0)
                {
                     if (!TeleportToLightAfterDecom && Map.DecontaminationState == DecontaminationState.Finish)
                     { 
                         ev.Player.Teleport(Room.List.Where(r => r.Zone is not ZoneType.LightContainment && !ExcludedRooms.Contains(r.Type)).GetRandomValue());
                     }
                     else
                     {
                         Room randomRoom = rooms[Base.GetRandomNumber.GetRandomInt(rooms.Count)];
                         Vector3 teleportPosition = randomRoom.Position + Vector3.up;
                         ev.Player.Position = teleportPosition;
                     }
                }
            });
        }
    }
}