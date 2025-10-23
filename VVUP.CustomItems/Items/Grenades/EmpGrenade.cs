using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Handlers;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using Attachment = InventorySystem.Items.Firearms.Attachments.Components.Attachment;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class EmpGrenade : CustomGrenade, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override uint Id { get; set; } = 30;
        public override string Name { get; set; } = "<color=#6600CC>PB-42</color>";

        public override string Description { get; set; } =
            "When detonating, lights will be turned off and doors will be opened";

        public override float Weight { get; set; } = 1.15f;
        public override bool ExplodeOnCollision { get; set; } = true;
        public override float FuseTime { get; set; } = 1.5f;

        public bool OpenLockedDoors { get; set; } = false;
        public bool OpenKeycardDoors { get; set; } = true;
        public bool DisableTeslas { get; set; } = true;
        public float Duration { get; set; } = 20;
        
        [CanBeNull]
        public List<DoorType> BlackListedDoors { get; set; } = new()
        {
            DoorType.Scp079First,
            DoorType.Scp079Second,
        };
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideSurfaceNuke,
                },
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Offset = new Vector3(-3.25f, 1, -0.78f),
                    Room = RoomType.Hcz096,
                }
            },
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 191);
        
        private static readonly List<Room> _lockedRooms079 = new();

        private readonly List<Door> _lockedDoors = new();

        private readonly List<TeslaGate> _disabledTeslaGates = new();
        
        protected override void SubscribeEvents()
        {
            Scp079.TriggeringDoor += OnInteractingDoor;

            if (DisableTeslas)
                PlayerEvent.TriggeringTesla += OnTriggeringTesla;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Scp079.TriggeringDoor -= OnInteractingDoor;

            if (DisableTeslas)
                PlayerEvent.TriggeringTesla -= OnTriggeringTesla;

            base.UnsubscribeEvents();
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            Room room = Room.FindParentRoom(ev.Projectile.GameObject);
            TeslaGate? gate = null;

            Log.Debug($"VVUP Custom Items: EMP Grenade, {ev.Projectile.GameObject.transform.position} - {room.Position} - {Room.List.Count}");

            _lockedRooms079.Add(room);

            room.TurnOffLights(Duration);

            if (DisableTeslas)
            {
                foreach (TeslaGate teslaGate in TeslaGate.AllGates)
                {
                    if (Room.FindParentRoom(teslaGate.gameObject) == room)
                    {
                        Log.Debug($"VVUP Custom Items: EMP Grenade, disabling {teslaGate}");
                        _disabledTeslaGates.Add(teslaGate);
                        gate = teslaGate;
                        break;
                    }
                }
            }

            Log.Debug($"VVUP Custom Items: EMP Grenade, {room.Doors.Count} - {room.Type}");

            foreach (Door door in room.Doors)
            {
                if (door == null ||
                    BlackListedDoors.Contains(door.Type) ||
                    (door.DoorLockType != 0 && !OpenLockedDoors) ||
                    (door.RequiredPermissions != DoorPermissionFlags.None && !OpenKeycardDoors) ||
                    door.Type.IsElevator())
                    return;

                Log.Debug($"VVUP Custom Items: EMP Grenade, Opening and locking {door}");

                door.IsOpen = true;
                door.ChangeLock(DoorLockType.NoPower);

                if (!_lockedDoors.Contains(door))
                    _lockedDoors.Add(door);

                Timing.CallDelayed(Duration, () =>
                {
                    door.Unlock();
                    _lockedDoors.Remove(door);
                });
            }

            foreach (PlayerAPI player in PlayerAPI.List)
            {
                if (player.Role.Is(out Scp079Role scp079))
                {
                    if (scp079.Camera != null && scp079.Camera.Room == room)
                    {
                        Log.Debug($"VVUP Custom Items: EMP Grenade, disconnecting {player.Nickname} for {Duration} seconds");
                        scp079.LoseSignal(Duration);
                    }
                }

                if (player.CurrentRoom != room)
                    continue;

                foreach (Exiled.API.Features.Items.Item item in player.Items)
                {
                    switch (item)
                    {
                        case Radio radio:
                            radio.IsEnabled = false;
                            break;
                        case Flashlight flashlight:
                            flashlight.IsEmittingLight = false;
                            break;
                        
                        case Firearm firearm:
                        {
                            foreach (Attachment attachment in firearm.Attachments)
                            {
                                if (attachment.Name == AttachmentName.Flashlight)
                                    attachment.IsEnabled = false;
                            }
                            break;
                        }
                    }
                }
            }

            Timing.CallDelayed(Duration, () =>
            {
                try
                {
                    _lockedRooms079.Remove(room);
                }
                catch (Exception ev)
                {
                    Log.Debug($"VVUP Custom Items: EMP Grenade, Removing Locked Room: {ev}");
                }

                if (gate != null)
                {
                    try
                    {
                        _disabledTeslaGates.Remove(gate);
                    }
                    catch (Exception ev)
                    {
                        Log.Debug($"VVUP Custom Items: EMP Grenade, Removing Disabled Tesla: {ev}");
                    }
                }
            });
        }

        private void OnInteractingDoor(TriggeringDoorEventArgs ev)
        {
            if (_lockedDoors.Contains(ev.Door))
            {
                Log.Debug($"VVUP Custom Items: EMP Grenade, {ev.Door} is currently disabled");
                ev.IsAllowed = false;
            }
        }

        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            foreach (TeslaGate gate in TeslaGate.AllGates)
            {
                if (Room.FindParentRoom(gate.gameObject) == ev.Player.CurrentRoom && _disabledTeslaGates.Contains(gate))
                {
                    Log.Debug($"VVUP Custom Items: EMP Grenade, {gate} is currently disabled");
                    ev.IsAllowed = false;
                }
            }
        }
    }
}