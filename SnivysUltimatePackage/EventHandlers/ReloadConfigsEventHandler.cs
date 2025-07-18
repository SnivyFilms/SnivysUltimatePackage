﻿using Exiled.API.Features;

namespace SnivysUltimatePackage.EventHandlers
{
    public class ReloadConfigsEventHandler
    {
        public Plugin Plugin;
        public ReloadConfigsEventHandler(Plugin plugin) => Plugin = plugin;
        public static void OnReloadingConfigs()
        {
            Log.Info("VVUP: Reloading configs for Snivy's Ultimate Plugin Package");
            Plugin.Instance.Config.LoadConfigs();
            Log.Info("VVUP: Configs reloaded successfully");
        }
    }
}