﻿using Exiled.API.Enums;
using Exiled.API.Features;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using Door = Exiled.API.Features.Doors.Door;

namespace SnivysUltimatePackage.EventHandlers
{
    public class RoundStartEventHandlers(Plugin plugin)
    {
        public Plugin Plugin = plugin;

        public void OnRoundStarted()
        {
            if (Plugin.Instance.RoundStartEventHandlers == null || !Plugin.Instance.Config.RoundStartConfig.IsEnabled)
                return;
            if (Plugin.Instance.Config.RoundStartConfig.EscapeDoorOpen)
            {
                Timing.CallDelayed(1.5f, () =>
                {
                    Log.Debug("VVUP Round Start Events: Opening Escape Final Door on Surface");
                    var escapeDoor = DoorType.EscapeFinal;
                    Door door = Door.Get(escapeDoor);
                    door.IsOpen = true;
                });
            }

            if (!Plugin.Instance.Config.RoundStartConfig.EscapeDoorLock)
            {
                Timing.CallDelayed(1.5f, () =>
                {
                    Log.Debug("VVUP Round Start Events: Unlocking Escape Final Door on Surface");
                    var escapeDoor = DoorType.EscapeFinal;
                    Door door = Door.Get(escapeDoor);
                    if (door.IsLocked)
                        door.ChangeLock(DoorLockType.AdminCommand);
                });
            }

            if (Plugin.Instance.Config.RoundStartConfig.DecontaminationChanges)
            {
                Log.Debug("VVUP Round Start Events: Changing Decontamination Time");
                Decontamination.Offset = Plugin.Instance.Config.RoundStartConfig.DecontaminationTime;
            }
        }
    }
}