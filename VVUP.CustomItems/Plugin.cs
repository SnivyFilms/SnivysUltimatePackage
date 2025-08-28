using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Loader;
using UserSettings.ServerSpecific;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems
{
    public class Plugin : Plugin<CustomItemsConfig>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Custom Items";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.CI";
        public override Version Version { get; } = new Version(3, 0, 4);
        public override Version RequiredExiledVersion { get; } = new Version(9, 8, 1);
        public SsssEventHandlers SsssEventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP CI: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }
            CustomItem.RegisterItems(overrideClass: Instance.Config);
            SsssEventHandlers = new SsssEventHandlers(this);
            Player.Verified += SsssEventHandlers.OnVerified;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += SsssEventHandlers.OnSettingValueReceived;
            Base.Plugin.Instance.VvupCi = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomItem.UnregisterItems();
            Player.Verified -= SsssEventHandlers.OnVerified;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= SsssEventHandlers.OnSettingValueReceived;
            SsssEventHandlers = null;
            Base.Plugin.Instance.VvupCi = false;
            Instance = null;
            base.OnDisabled();
        }
    }
}