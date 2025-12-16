using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Interactables.Interobjects.DoorUtils;
using LightContainmentZoneDecontamination;
using MEC;
using UnityEngine;
using VVUP.ServerEvents.ServerEventsConfigs;
using CheckpointDoor = Exiled.API.Features.Doors.CheckpointDoor;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    public class FreezingTemperaturesEventHandlers
    {
        private static CoroutineHandle _freezingTemperaturesHandle;
        private static FreezingTemperaturesConfig _config;
        private static bool _fteStarted;
        private static double _previousDecomTime;

        public FreezingTemperaturesEventHandlers()
        {
            Log.Debug("VVUP Server Events, Freezing Temperatures: Checking if Freezing Temperatures Event has already started");
            if (_fteStarted) return;
            _config = Plugin.Instance.Config.FreezingTemperaturesConfig;
            Plugin.ActiveEvent += 1;
            _fteStarted = true;
            LabApi.Features.Wrappers.Cassie.Message(_config.StartEventCassieMessage, _config.StartEventCassieText);
            _previousDecomTime = DecontaminationController.Singleton.RoundStartTime;
            DecontaminationController.Singleton.NetworkRoundStartTime = -1.0;
            _freezingTemperaturesHandle = Timing.RunCoroutine(FreezingTemperaturesTiming());
            Log.Debug("VVUP Server Events, Freezing Temperatures: Stopping regular decontamination, starting custom freezing temperatures system");
        }

        private static IEnumerator<float> FreezingTemperaturesTiming()
        {
            Log.Debug("VVUP Server Events, Freezing Temperatures: Doing a quick check to make sure the event isn't started properly");
            if (!_fteStarted)
                yield break;

            Log.Debug("VVUP Server Events, Freezing Temperatures: Making and defining the SCP-244 instance that will be used to freeze each zone");
            Scp244 freezing = (Scp244)Item.Create(ItemType.SCP244a);
            freezing.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            freezing.Primed = true;
            freezing.MaxDiameter = 10;
            Log.Debug(
                $"VVUP Server Events, Freezing Temperatures: Info about SCP-244 that will be used. Scale: {freezing.Scale}, Primed: {freezing.Primed}, Max Diameter {freezing.MaxDiameter} (decently sure this doesnt change the cloud size but idk");

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.LightTimeWarning} seconds");
            yield return Timing.WaitForSeconds(_config.LightTimeWarning);
            Log.Debug("VVUP Server Events, Freezing Temperatures: Showing Light Half Time Remaining Message");
            LabApi.Features.Wrappers.Cassie.Message(_config.LightHalfTimeRemainingWarningMessage,
                _config.LightHalfTimeRemainingWarningText);

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.LightCompleteFreezeTime} seconds");
            yield return Timing.WaitForSeconds(_config.LightCompleteFreezeTime);
            Log.Debug("VVUP Server Events, Freezing Temperatures: Showing Light Frozen Over Message");
            LabApi.Features.Wrappers.Cassie.Message(_config.LightFrozenOverMessage, _config.LightFrozenOverText);

            Log.Debug("VVUP Server Events, Freezing Temperatures: Locking and closing Light Containment Zone Elevators");
            if (!Lift.Get(ElevatorType.LczA).IsLocked)
                Lift.Get(ElevatorType.LczA).ChangeLock(DoorLockReason.AdminCommand);
            if (!Lift.Get(ElevatorType.LczB).IsLocked)
                Lift.Get(ElevatorType.LczB).ChangeLock(DoorLockReason.AdminCommand);

            Log.Debug("VVUP Server Events, Freezing Temperatures: Locking and closing each door in Light Containment Zone");
            Log.Debug("VVUP Server Events, Freezing Temperatures: Spawning SCP-244s in each room in Light Containment Zone");
            foreach (Room rooms in Room.List)
            {
                if (rooms.Zone == ZoneType.LightContainment)
                {
                    foreach (Door door in Door.List)
                    {
                        if (door.Zone == ZoneType.LightContainment)
                        {
                            if (!door.IsLocked)
                                door.ChangeLock(DoorLockType.AdminCommand);
                            if (door.IsOpen)
                                door.IsOpen = false;
                        }
                    }

                    freezing.CreatePickup(rooms.Position);
                }
            }

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.KillPlayersInZoneAfterTime} seconds");
            yield return Timing.WaitForSeconds(_config.KillPlayersInZoneAfterTime);
            foreach (Player player in Player.List)
            {
                if (player.Zone == ZoneType.LightContainment)
                {
                    Log.Debug($"VVUP Server Events, Freezing Temperatures: Killing {player.Nickname} as they are in Light Containment Zone");
                    player.Kill(_config.PlayersDeathReason);
                }
            }

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.HeavyTimeWarning} seconds");
            yield return Timing.WaitForSeconds(_config.HeavyTimeWarning);
            Log.Debug("VVUP Server Events, Freezing Temperatures: Showing Heavy Half Time Remaining Message");
            LabApi.Features.Wrappers.Cassie.Message(_config.HeavyHalfTimeRemainingWarningMessage,
                _config.HeavyHalfTimeRemainingWarningText);

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.HeavyCompleteFreezeTime} seconds");
            yield return Timing.WaitForSeconds(_config.HeavyCompleteFreezeTime);
            Log.Debug("VVUP Server Events, Freezing Temperatures: Showing Heavy Frozen over Cassie Message");
            LabApi.Features.Wrappers.Cassie.Message(_config.HeavyFrozenOverMessage, _config.HeavyFrozenOverText);

            Log.Debug("VVUP Server Events, Freezing Temperatures: Locking Nuke & SCP-049 Elevators");
            if (!Lift.Get(ElevatorType.Nuke).IsLocked)
                Lift.Get(ElevatorType.Nuke).ChangeLock(DoorLockReason.AdminCommand);
            if (!Lift.Get(ElevatorType.Scp049).IsLocked)
                Lift.Get(ElevatorType.Scp049).ChangeLock(DoorLockReason.AdminCommand);
            Log.Debug("VVUP Server Events, Freezing Temperatures: Closing and locking all Heavy Containment Zone Doors");
            Log.Debug("VVUP Server Events, Freezing Temperatures: Spawning SCP-244's in each room");
            foreach (Room rooms in Room.List)
            {
                if (rooms.Zone == ZoneType.HeavyContainment)
                {
                    foreach (Door door in Door.List)
                    {
                        if (door.Zone == ZoneType.HeavyContainment)
                        {
                            if (!door.IsLocked)
                                door.ChangeLock(DoorLockType.AdminCommand);
                            if (door.IsOpen)
                                door.IsOpen = false;
                        }
                        else if (door is CheckpointDoor checkpointDoor)
                        {
                            checkpointDoor.IsOpen = false;
                            if (!checkpointDoor.IsLocked)
                                checkpointDoor.ChangeLock((DoorLockType)DoorLockReason.AdminCommand);
                        }
                    }

                    freezing.CreatePickup(rooms.Position);
                }
            }

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.KillPlayersInZoneAfterTime} seconds");
            yield return Timing.WaitForSeconds(_config.KillPlayersInZoneAfterTime);
            foreach (Player player in Player.List)
            {
                if (player.Zone == ZoneType.HeavyContainment)
                {
                    Log.Debug($"VVUP Server Events, Freezing Temperatures: Killing {player.Nickname} as they are in Heavy Containment Zone");
                    player.Kill(_config.PlayersDeathReason);
                }
            }

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.EntranceTimeWarning} seconds");
            yield return Timing.WaitForSeconds(_config.EntranceTimeWarning);
            Log.Debug("VVUP Server Events, Freezing Temperatures: Showing Entrance Half Time Remaining Warning");
            LabApi.Features.Wrappers.Cassie.Message(_config.EntranceHalfTimeRemainingWarningMessage,
                _config.EntranceHalfTimeRemainingWarningText);

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.EntranceCompleteFreezeTime} seconds");
            yield return Timing.WaitForSeconds(_config.EntranceCompleteFreezeTime);
            Log.Debug("VVUP Server Events, Freezing Temperatures: Showing Entrance Frozen Over Message");
            LabApi.Features.Wrappers.Cassie.Message(_config.EntranceFrozenOverMessage, _config.EntranceFrozenOverText);

            Log.Debug("VVUP Server Events, Freezing Temperatures: Locking Gate A and B elevators");
            if (!Lift.Get(ElevatorType.GateA).IsLocked)
                Lift.Get(ElevatorType.GateA).ChangeLock(DoorLockReason.AdminCommand);
            if (!Lift.Get(ElevatorType.GateB).IsLocked)
                Lift.Get(ElevatorType.GateB).ChangeLock(DoorLockReason.AdminCommand);
            Log.Debug("VVUP Server Events, Freezing Temperatures: Closing and Locking all doors in Entrance Zone");
            Log.Debug("VVUP Server Events, Freezing Temperatures: Spawning SCP-244 in each Entrance Zone Room");
            foreach (Room rooms in Room.List)
            {
                if (rooms.Zone == ZoneType.Entrance)
                {
                    foreach (Door door in Door.List)
                    {
                        if (door.Zone == ZoneType.Entrance)
                        {
                            if (!door.IsLocked)
                                door.ChangeLock(DoorLockType.AdminCommand);
                            if (door.IsOpen)
                                door.IsOpen = false;
                        }
                    }

                    freezing.CreatePickup(rooms.Position);
                }
            }

            Log.Debug($"VVUP Server Events, Freezing Temperatures: Waiting {_config.KillPlayersInZoneAfterTime} seconds");
            yield return Timing.WaitForSeconds(_config.KillPlayersInZoneAfterTime);
            foreach (Player player in Player.List)
            {
                if (player.Zone == ZoneType.Entrance)
                {
                    Log.Debug($"VVUP Server Events, Freezing Temperatures: Killing {player.Nickname} as they are in Entrance Zone");
                    player.Kill(_config.PlayersDeathReason);
                }
            }

            Log.Debug("VVUP Server Events, Freezing Temperatures: Ending the event as this event has ran through all it needs to do");
            EndEvent();
        }

        public static void EndEvent()
        {
            if (!_fteStarted) return;
            _fteStarted = false;
            Plugin.ActiveEvent -= 1;
            DecontaminationController.Singleton.RoundStartTime = _previousDecomTime;
            Timing.KillCoroutines(_freezingTemperaturesHandle);
        }
    }
}