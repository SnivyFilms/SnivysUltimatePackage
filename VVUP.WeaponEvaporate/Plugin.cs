using System;
using System.Linq;
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
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.WE";
        public override Version Version { get; } = new Version(3, 2, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 1);
        public EventHandlers EventHandlers;
        
        public override void OnEnabled()
        {
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP WE: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }

            EventHandlers = new EventHandlers(this);
            Player.Shot += EventHandlers.OnShot;
            Player.Dying += EventHandlers.OnDying;
            Player.Hurt -= EventHandlers.OnHurt;
            Instance = this;
            Base.Plugin.Instance.VvupWe = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupWe = false;
            Player.Dying -= EventHandlers.OnDying;
            Player.Shot -= EventHandlers.OnShot;
            Player.Hurt -= EventHandlers.OnHurt;
            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}