using System;
using System.Linq;
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
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.CE";
        public override Version Version { get; } = new Version(3, 5, 6);
        public override Version RequiredExiledVersion { get; } = new Version(9, 12, 2);
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