using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;

namespace VVUP.CustomEscapes
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Custom Escapes";
        public override string Prefix { get; } = "VVUP.CE";
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
                    .Value);
        public EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP CE: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }
            
            EventHandlers = new EventHandlers(this);
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Player.Escaping += EventHandlers.OnDefaultEscape;
            Base.Plugin.Instance.VvupCe = true;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupCe = false;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Player.Escaping -= EventHandlers.OnDefaultEscape;
            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}