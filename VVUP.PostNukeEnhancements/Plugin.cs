using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;

namespace VVUP.PostNukeEnhancements
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Post Nuke Enhancements";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.PNE";
        public override Version Version { get; } = new Version(3, 5, 6);
        public override Version RequiredExiledVersion { get; } = new Version(9, 12, 2);
        public EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP PNE: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }
            
            EventHandlers = new EventHandlers(this);
            Exiled.Events.Handlers.Warhead.Detonated += EventHandlers.OnNukeDetonated;
            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            Base.Plugin.Instance.VvupPne = true;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupPne = false;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnNukeDetonated;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}