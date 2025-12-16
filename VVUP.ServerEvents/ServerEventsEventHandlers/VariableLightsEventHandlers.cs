using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using VVUP.ServerEvents.ServerEventsConfigs;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    public class VariableLightsEventHandlers
    {
        private static CoroutineHandle _lightChangingHandle;
        private static VariableLightsConfig _config;
        private static bool _vleStarted;

        public VariableLightsEventHandlers()
        {
            Log.Debug("VVUP Server Events, Variable Lights: Checking to see if Variable Lights Event has already started");
            if (_vleStarted) return;
            _config = Plugin.Instance.Config.VariableLightsConfig;
            Plugin.ActiveEvent += 1;
            _vleStarted = true;
            Map.ResetLightsColor();
            LabApi.Features.Wrappers.Cassie.Message(_config.StartEventCassieMessage, _config.StartEventCassieText);
            _lightChangingHandle = Timing.RunCoroutine(VariableLightsTiming());
        }

        private static IEnumerator<float> VariableLightsTiming()
        {
            Log.Debug("VVUP Server Events, Variable Lights: Checking if Variable Lights Event has started improperly");
            if (!_vleStarted)
            {
                Log.Warn("VVUP Server Events, Variable Lights: Variable Lights Event has started improperly, ending event.");
                Map.ResetLightsColor();
                yield break;
            }

            for (;;)
            {
                Log.Debug("VVUP Server Events, Variable Lights: Checking if config is set to allow color channing or not");
                if (!_config.ColorChanging)
                {
                    Log.Debug("VVUP Server Events, Variable Lights: Color changing is disabled, changing brightness only");
                    if (_config.DifferentLightsPerRoom)
                    {
                        Log.Debug("VVUP Server Events, Variable Lights: Different lights per room is enabled, changing brightness");
                        foreach (Room room in Room.List)
                            room.Color = new Color(room.Color.r, room.Color.g, room.Color.b, Base.GetRandomNumber.GetRandomFloat(1));
                    }
                    else
                    {
                        Log.Debug(
                            "VVUP Server Events, Variable Lights: Different lights per room is disabled, setting brightness to be the same across rooms");
                            Map.ChangeLightsColor(new Color(1, 1, 1, Base.GetRandomNumber.GetRandomFloat(1)));
                    }
                }
                else
                {
                    Log.Debug("VVUP Server Events, Variable Lights: Color changing is enabled");
                    if (_config.DifferentLightsPerRoom)
                    {
                        Log.Debug("VVUP Server Events, Variable Lights: Different room lights is enabled, setting different lights per room");
                        foreach (Room room in Room.List)
                            room.Color = new Color(Base.GetRandomNumber.GetRandomFloat(1), Base.GetRandomNumber.GetRandomFloat(1),
                                Base.GetRandomNumber.GetRandomFloat(1), Base.GetRandomNumber.GetRandomFloat(1));
                    }
                    else
                    {
                        Log.Debug("VVUP Server Events, Variable Lights: Different room lights is disabled, setting the same lights per room");
                        Map.ChangeLightsColor(new Color(
                            Base.GetRandomNumber.GetRandomFloat(1), 
                            Base.GetRandomNumber.GetRandomFloat(1),
                            Base.GetRandomNumber.GetRandomFloat(1), 
                            Base.GetRandomNumber.GetRandomFloat(1)));
                    }
                }

                Log.Debug($"VVUP Server Events, Variable Lights: Waiting for {_config.TimeForChange} seconds");
                yield return Timing.WaitForSeconds(_config.TimeForChange);
            }
        }

        public static void EndEvent()
        {
            if (!_vleStarted) return;
            _vleStarted = false;
            Log.Debug("VVUP Server Events, Variable Lights: Killing Coroutine for lights");
            Timing.KillCoroutines(_lightChangingHandle);
            Map.ResetLightsColor();
            Plugin.ActiveEvent -= 1;
        }
    }
}