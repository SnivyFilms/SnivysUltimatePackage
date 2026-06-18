using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.WeaponEvaporate
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Weapon Evaporate";
        public override string Prefix { get; } = "VVUP.WE";
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
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP WE: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }

            EventHandlers = new EventHandlers(this);
            Player.Dying += EventHandlers.OnDying;
            Instance = this;
            Base.Plugin.Instance.VvupWe = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupWe = false;
            Player.Dying -= EventHandlers.OnDying;
            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}