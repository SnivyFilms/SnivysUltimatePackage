using System;
using System.Linq;
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
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.OFCSE";
        public override Version Version { get; } = new Version(3, 3, 1);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 2);

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