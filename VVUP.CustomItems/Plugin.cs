using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Loader;
using HarmonyLib;
using UserSettings.ServerSpecific;
using Config = VVUP.CustomItems.Configs.Config;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Custom Items";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.CI";
        public override Version Version { get; } = new Version(3, 4, 3);
        public override Version RequiredExiledVersion { get; } = new Version(9, 10, 0);
        public SsssEventHandlers SsssEventHandlers;
        private Harmony _harmony;

        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP CI: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }
            _harmony = new Harmony("vvup.customitems");
            _harmony.PatchAll();
            CustomItem.RegisterItems(overrideClass: Instance.Config.CustomItemsConfig);
            SsssEventHandlers = new SsssEventHandlers(this);
            Player.Verified += SsssEventHandlers.OnVerified;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += SsssEventHandlers.OnSettingValueReceived;
            Base.Plugin.Instance.VvupCi = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _harmony.UnpatchAll("vvup.customitems");
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