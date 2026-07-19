using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;

namespace VVUP.PostNukeEnhancements
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Post Nuke Enhancements";
        public override string Prefix { get; } = "VVUP.PNE";
        public override string Author =>
            Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyCompanyAttribute>()?
                .Company ?? "Unknown";
        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version;
        public override Version RequiredExiledVersion =>
            Version.Parse(
                Assembly.GetExecutingAssembly()
                    .GetCustomAttributes<AssemblyMetadataAttribute>()
                    .First(x => x.Key == "RequiredExiledVersion")
                    .Value);        public EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP PNE: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }
            
            EventHandlers = new EventHandlers(this);
            Exiled.Events.Handlers.Warhead.Detonated += EventHandlers.OnNukeDetonated;
            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            Base.Plugin.Instance.VvupPne = true;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupPne = false;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnNukeDetonated;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}