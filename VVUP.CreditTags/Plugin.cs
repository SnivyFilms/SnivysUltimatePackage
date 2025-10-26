using System;
using System.Linq;
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
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.CT";
        public override Version Version { get; } = new Version(3, 4, 1);
        public override Version RequiredExiledVersion { get; } = new Version(9, 10, 0);
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