using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Loader;

namespace VVUP.OperationCrossfireServerEvent
{
    public class Plugin : Plugin<OperationCrossfireConfig>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Lower;
        public static Plugin Instance;
        public override string Name => "VVUP: Operation Crossfire Server Event";
        public override string Prefix { get; } = "VVUP.OFCSE";
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
        public OperationCrossfireEventHandlers OperationCrossfireEventHandlers { get; set; }

        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP OCF: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.CI"))
            {
                Log.Error("VVUP HK: Custom Items Module is not present, disabling module");
                base.OnDisabled();
                return;
            }
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.SE"))
            {
                Log.Error("VVUP OCF: Server Event Module is not present, disabling module");
                base.OnDisabled();
                return;
            }
            CustomItem.RegisterItems(overrideClass: Instance.Config);
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomItem.UnregisterItems();
            Instance = null;
            base.OnDisabled();
        }
    }
}