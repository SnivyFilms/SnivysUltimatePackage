using System.Collections.Generic;
using Exiled.API.Features;
using VVUP.ServerEvents.ServerEventsConfigs;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    internal class ShortEventHandlers
    {
        private static ShortConfig _config;
        private static bool _seStarted;
        
        public ShortEventHandlers()
        {
            Log.Debug("VVUP Server Events, Short People: Checking if Short People Event has already started");
            if (_seStarted) return;
            _config = Plugin.Instance.Config.ShortConfig;
            Plugin.ActiveEvent += 1;
            Log.Debug("VVUP Server Events, Short People: Adding On Changing Role SE Event Handlers");
            Exiled.Events.Handlers.Player.ChangingRole += Plugin.Instance.ServerEventsMainEventHandler.OnRoleSwapSE;
            _seStarted = true;
            foreach (var player in Player.List)
            {
                var startingItems = GetStartingItems(_config.StartingItems);
                foreach (var item in startingItems)
                {
                    Log.Debug($"VVUP Server Events, Short People: Adding {item} to {player.Nickname}");
                    player.AddItem(item);
                }
                player.Scale = new UnityEngine.Vector3(GetPlayerSize(), GetPlayerSize(), GetPlayerSize());
                Log.Debug($"VVUP Server Events, Short People: Set {player.Nickname} size to {GetPlayerSize()}");
            }
            LabApi.Features.Wrappers.Cassie.Message(_config.StartEventCassieMessage, _config.StartEventCassieText);
        }
        
        public static float GetPlayerSize()
        {
            Log.Debug("VVUP Server Events, Short People: Getting Config Defined Player Size");
            return _config.PlayerSize;
        }

        private static List<ItemType> GetStartingItems(List<ItemType> items)
        {
            Log.Debug("VVUP Server Events, Short People: Getting config defined starting items");
            return items;
        }
        public static void EndEvent()
        {
            if (!_seStarted) return;
            LabApi.Features.Wrappers.Cassie.Message(_config.EndEventCassieMessage, _config.EndEventCassieText);
            Log.Debug("VVUP Server Events, Short People: Unregistering ChangingRole (SE) Event Handlers");
            Exiled.Events.Handlers.Player.ChangingRole -= Plugin.Instance.ServerEventsMainEventHandler.OnRoleSwapSE;
            _seStarted = false;
            Plugin.ActiveEvent -= 1;
        }
    }
}
