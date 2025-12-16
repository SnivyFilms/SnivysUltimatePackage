using Exiled.API.Features;
using MEC;
using VVUP.ServerEvents.ServerEventsConfigs;
using Maps = Exiled.Events.Handlers.Map;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    public class BlackoutEventHandlers
    {
        private static BlackoutConfig _config;
        private static bool _boeStarted;
        public BlackoutEventHandlers()
        {
            Log.Debug("VVUP Server Events, Blackout: Checking if Blackout Event has already started");
            if (_boeStarted) return;
            _config = Plugin.Instance.Config.BlackoutConfig;
            Plugin.ActiveEvent += 1;
            if (_config.GeneratorEndsEvent)
            {
                Log.Debug("VVUP Server Events, Blackout: Enabling the Generator Activation Event Handlers");
                Maps.GeneratorActivating += Plugin.Instance.ServerEventsMainEventHandler.OnGeneratorEngagedBOE;
            }
            
            _boeStarted = true;
            Log.Debug("VVUP Server Events, Blackout: Turning off the lights");
            if (_config.FlickerLights)
            {
                foreach (Room room in Room.List)
                {
                    room.RoomLightController.ServerFlickerLights(_config.FlickerDuration);
                }

                Timing.CallDelayed(_config.FlickerDuration, () => Map.TurnOffAllLights(432000));
            }
            else
                Map.TurnOffAllLights(432000);
            foreach (var player in Player.List)
            {
                Log.Debug($"VVUP Server Events, Blackout: Adding {_config.BlackoutEventStartingItem} to {player.Nickname}");
                player.AddItem(_config.BlackoutEventStartingItem);
            }

            LabApi.Features.Wrappers.Cassie.Message(_config.StartEventCassieMessage, _config.StartEventCassieText);
        }

        public static void EndEvent()
        {
            if (!_boeStarted) return;
            LabApi.Features.Wrappers.Cassie.Message(_config.EndEventCassieMessage, _config.EndEventCassieText);
            if (_config.GeneratorEndsEvent)
            {
                Log.Debug("VVUP Server Events, Blackout: Disabling Generator Engaged Event Handler");
                Maps.GeneratorActivating -= Plugin.Instance.ServerEventsMainEventHandler.OnGeneratorEngagedBOE;
            }
            Log.Debug("VVUP Server Events, Blackout: Turning on the lights");
            Map.TurnOffAllLights(1);
            _boeStarted = false;
            Plugin.ActiveEvent -= 1;
        }
    }
}