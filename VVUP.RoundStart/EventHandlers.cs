using Exiled.API.Enums;
using Exiled.API.Features;
using LabApi.Features.Wrappers;
using MEC;
using Door = Exiled.API.Features.Doors.Door;

namespace VVUP.RoundStart
{
    public class EventHandlers
    {
        public Plugin Plugin;
        public EventHandlers(Plugin plugin) => Plugin = plugin;

        public void OnRoundStarted()
        {
            if (Plugin.Instance.Config.EscapeDoorOpen)
            {
                Timing.CallDelayed(1.5f, () =>
                {
                    Log.Debug("VVUP Round Start Events: Opening Escape Final Door on Surface");
                    var escapeDoor = DoorType.EscapeFinal;
                    Door door = Door.Get(escapeDoor);
                    door.IsOpen = true;
                });
            }

            if (!Plugin.Instance.Config.EscapeDoorLock)
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

            if (Plugin.Instance.Config.DecontaminationChanges)
            {
                Log.Debug("VVUP Round Start Events: Changing Decontamination Time");
                Decontamination.Offset = Plugin.Instance.Config.DecontaminationTime;
            }
        }
    }
}