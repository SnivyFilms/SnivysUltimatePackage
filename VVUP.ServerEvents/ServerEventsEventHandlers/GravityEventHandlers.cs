using Exiled.API.Features;
using VVUP.ServerEvents.ServerEventsConfigs;
using PlayerLab = LabApi.Features.Wrappers.Player;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    internal class GravityEventHandlers
    {
        private static GravityConfig _config;
        private static bool _geStarted;
        
        public GravityEventHandlers()
        {
            Log.Debug("VVUP Server Events, Low Gravity: Checking if Low Gravity Event has already started");
            if (_geStarted) return;
            _config = Plugin.Instance.Config.GravityConfig;
            Plugin.ActiveEvent += 1;
            Log.Debug("VVUP Server Events, Low Gravity: Adding On Changing Role SE Event Handlers");
            Exiled.Events.Handlers.Player.ChangingRole += Plugin.Instance.ServerEventsMainEventHandler.OnRoleSwapGE;
            _geStarted = true;
            foreach (var player in Player.List)
            {
                PlayerLab.Get(player.NetworkIdentity)!.Gravity = _config.GravityChanges;
                Log.Debug($"VVUP Server Events, Low Gravity: Set {player.Nickname} size to {_config.GravityChanges}");
            }
            Cassie.MessageTranslated(_config.StartEventCassieMessage, _config.StartEventCassieText);
        }
        
        public static void EndEvent()
        {
            if (!_geStarted) return;
            Log.Debug("VVUP Server Events, Low Gravity: Unregistering ChangingRole (GE) Event Handlers");
            Exiled.Events.Handlers.Player.ChangingRole -= Plugin.Instance.ServerEventsMainEventHandler.OnRoleSwapGE;
            _geStarted = false;
            Plugin.ActiveEvent -= 1;
        }
    }
}
