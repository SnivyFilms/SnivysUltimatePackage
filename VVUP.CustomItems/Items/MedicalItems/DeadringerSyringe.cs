using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using VVUP.Base;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.MedicalItems
{
    [CustomItem(ItemType.Adrenaline)]
    public class DeadringerSyringe : CustomItem, ICustomItemGlow
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
        [Description("The delay in seconds before the main effects are applied")]
        public float Delay { get; set; } = 3f;
        [Description("The effects that are applied as soon at the item is used")]
        public List<ApplyEffects> SideEffectsFirst { get; set; } = new()
        {
            new()
            {
                EffectType = EffectType.Blinded,
                Intensity = 1,
                Duration = 15f,
                AddDurationIfActive = true
            }
        };
        [Description("The effects that are applied after the delay")]
        public List<ApplyEffects> SideEffectsSecond { get; set; } = new()
        {
            new()
            {
                EffectType = EffectType.Flashed,
                Intensity = 1,
                Duration = 5f,
                AddDurationIfActive = true
            },
            new()
            {
                EffectType = EffectType.Invisible,
                Intensity = 1,
                Duration = 5f,
                AddDurationIfActive = true
            },
            new()
            {
                EffectType = EffectType.Ensnared,
                Intensity = 1,
                Duration = 5f,
                AddDurationIfActive = true
            },
            new()
            {
                EffectType = EffectType.Disabled,
                Intensity = 1,
                Duration = 60f,
                AddDurationIfActive = true
            },
            new()
            {
                EffectType = EffectType.Exhausted,
                Intensity = 1,
                Duration = 15f,
                AddDurationIfActive = true
            },
            new()
            {
                EffectType = EffectType.AmnesiaItems,
                Intensity = 1,
                Duration = 30f,
                AddDurationIfActive = true
            },
            new()
            {
                EffectType = EffectType.AmnesiaVision,
                Intensity = 1,
                Duration = 30f,
                AddDurationIfActive = true
            },
        };
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
                new()
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
        public bool HasCustomItemGlow { get; set; } = false;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 255, 255, 255);
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
            Log.Debug("VVUP Custom Items: Deadringer Syringe, Running methods");
            if (UseHints)
                ev.Player.ShowHint(OnUseMessage, OnUseMessageTimeDuration);
            else
                ev.Player.Broadcast((ushort)OnUseMessageTimeDuration, OnUseMessage);
            foreach (var effect in SideEffectsFirst)
            {
                Log.Debug(
                    $"VVUP Custom Items: Deadringer Syringe, applying {effect.EffectType} with intensity {effect.Intensity} and duration {effect.Duration} to {ev.Player.Nickname}");
                ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
            }
            Timing.CallDelayed(Delay, () =>
            {
                foreach (var effect in SideEffectsSecond)
                {
                    Log.Debug(
                        $"VVUP Custom Items: Deadringer Syringe, applying {effect.EffectType} with intensity {effect.Intensity} and duration {effect.Duration} to {ev.Player.Nickname}");
                    ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
                }
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