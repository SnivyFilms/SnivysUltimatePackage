using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using VVUP.ServerEvents.ServerEventsConfigs;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    public class AfterHoursEventHandlers
    {
        private static AfterHoursConfig _config;
        private static bool _ahStarted;
        private static CoroutineHandle _afterHoursHandle;
        public static bool AhTeslaAllowed = true;

        public AfterHoursEventHandlers()
        {
            Log.Debug("VVUP Server Events, After Hours: Checking if After Hours Event has already started");
            if (_ahStarted) return;
            _config = Plugin.Instance.Config.AfterHoursConfig;
            Plugin.ActiveEvent += 1;
            _ahStarted = true;
            Log.Debug("VVUP Server Events, After Hours: Dimming the lights, reducing tesla activation chance, reducing intercom time");
            Map.ChangeLightsColor(new Color(0.25f, 0.25f, 0.25f));
            PlayerEvent.TriggeringTesla += Plugin.Instance.ServerEventsMainEventHandler.OnTeslaActivationAh;
            Intercom.SpeechRemainingTime = _config.IntercomTime;
            LabApi.Features.Wrappers.Cassie.Message(_config.StartEventCassieMessage, _config.StartEventCassieText);
            foreach (var player in Player.List)
            {
                var startingItems = GetStartingItems(_config.StartingItems);
                foreach (var item in startingItems)
                {
                    Log.Debug($"VVUP Server Events, After Hours: Adding {item} to {player.Nickname}");
                    player.AddItem(item);
                }
            }
            _afterHoursHandle = Timing.RunCoroutine(AfterHoursTiming());
        }
        private static List<ItemType> GetStartingItems(List<ItemType> items)
        {
            Log.Debug("VVUP Server Events, After Hours: Getting config defined starting items");
            return items;
        }
        private static IEnumerator<float> AfterHoursTiming()
        {
            AhTeslaAllowed = Base.GetRandomNumber.GetRandomInt(101) <= _config.TeslaActivationChance;
            yield return Timing.WaitForSeconds(_config.TeslaActivationChanceCycle);
        }
        
        public static void EndEvent()
        {
            if (!_ahStarted) return;
            PlayerEvent.TriggeringTesla -= Plugin.Instance.ServerEventsMainEventHandler.OnTeslaActivationAh;
            Map.ResetLightsColor();
            _ahStarted = false;
            Timing.KillCoroutines(_afterHoursHandle);
            Plugin.ActiveEvent -= 1;
        }
    }
}