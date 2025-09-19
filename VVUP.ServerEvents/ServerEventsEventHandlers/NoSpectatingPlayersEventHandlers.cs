using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using VVUP.ServerEvents.ServerEventsConfigs;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    public class NoSpectatingPlayersEventHandlers
    {
        private static NoSpectatingPlayersConfig _config;
        private static bool _nspStarted;
        public NoSpectatingPlayersEventHandlers()
        {
            Log.Debug("VVUP Server Events, No Spectators: Checking if No Spectators Event has already started");
            if (_nspStarted) return;
            _nspStarted = true;
            _config = Plugin.Instance.Config.NoSpectatingPlayersConfig;
            Cassie.MessageTranslated(_config.StartEventCassieMessage, _config.StartEventCassieText);
            foreach (PlayerAPI player in PlayerAPI.List)
            {
                Log.Debug($"VVUP Server Events, No Spectators: Setting {player.Nickname} to not be spectatable");
                player.IsSpectatable = false;
            }
            Plugin.ActiveEvent += 1;
            Log.Debug("VVUP Server Events, No Spectators: Adding On Role Change event handler");
            PlayerEvent.ChangingRole += OnRoleChange;
        }
        
        private static void OnRoleChange(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole is RoleTypeId.Spectator or RoleTypeId.Overwatch or RoleTypeId.Filmmaker)
            {
                Log.Debug($"VVUP Server Events, No Spectators: Setting {ev.Player.Nickname} to be spectatable since they are dead");
                ev.Player.IsSpectatable = true;
            }
            else
            {
                Log.Debug($"VVUP Server Events, No Spectators: Setting {ev.Player.Nickname} to not be spectatable since they are alive");
                ev.Player.IsSpectatable = false;
            }
        }
        
        public static void EndEvent()
        {
            if (!_nspStarted) return;
            _nspStarted = false;
            Plugin.ActiveEvent -= 1;
            Log.Debug("VVUP Server Events, No Spectators: Disabling the On Role Change Event Handler");
            PlayerEvent.ChangingRole -= OnRoleChange;
            foreach (PlayerAPI player in PlayerAPI.List)
            {
                Log.Debug($"VVUP Server Events, No Spectators: Setting {player.Nickname} to be spectatable");
                player.IsSpectatable = true;
            }
        }
    }
}