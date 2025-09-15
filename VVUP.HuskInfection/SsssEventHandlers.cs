using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using UserSettings.ServerSpecific;

namespace VVUP.HuskInfection
{
    public class SsssEventHandlers
    {
        public Plugin Plugin;
        public SsssEventHandlers(Plugin plugin) => Plugin = plugin;

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (Plugin.Instance.SsssEventHandlers == null)
                return;
            if (!Plugin.Instance.Config.SsssEnabled)
                return;

            Log.Debug($"VVUP: Adding SSSS functions to {ev.Player.Nickname}");
            SsssHelper.SafeAppendSsssSettings();
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }
    }
}