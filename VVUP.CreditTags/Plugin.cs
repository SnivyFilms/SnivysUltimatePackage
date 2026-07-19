using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CreditTags
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Credit Tags";
        public override string Prefix { get; } = "VVUP.CT";
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
                Log.Error("VVUP CT: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }
            EventHandlers = new EventHandlers(this);
            Player.Verified += EventHandlers.OnVerified;
            Base.Plugin.Instance.VvupCt = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupCt = false;
            Player.Verified -= EventHandlers.OnVerified;
            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}