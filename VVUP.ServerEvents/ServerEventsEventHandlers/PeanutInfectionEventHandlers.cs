using Exiled.API.Features;
using VVUP.ServerEvents.ServerEventsConfigs;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    public class PeanutInfectionEventHandlers
    {
        private static PeanutInfectionConfig _config;
        private static bool _pieStarted;
        public PeanutInfectionEventHandlers()
        {
            Log.Debug("VVUP Server Events, Peanut Infection: Checking if Peanut Infection Event has already started");
            if (_pieStarted) return;
            _config = Plugin.Instance.Config.PeanutInfectionConfig;
            Plugin.ActiveEvent += 1;
            Log.Debug("VVUP Server Events, Peanut Infection: Adding Player Died Event PIE Handler");
            Player.Died += Plugin.Instance.ServerEventsMainEventHandler.OnKillingPIE;
            _pieStarted = true;
            LabApi.Features.Wrappers.Cassie.Message(_config.StartEventCassieMessage, _config.StartEventCassieText);
        }

        public static void EndEvent()
        {
            if (!_pieStarted) return;
            LabApi.Features.Wrappers.Cassie.Message(_config.EndEventCassieMessage, _config.EndEventCassieText);
            Log.Debug("VVUP Server Events, Peanut Infection: Removing Player Died Event PIE Handler");
            Player.Died -= Plugin.Instance.ServerEventsMainEventHandler.OnKillingPIE;
            _pieStarted = false;
            Plugin.ActiveEvent -= 1;
        }
    }
}