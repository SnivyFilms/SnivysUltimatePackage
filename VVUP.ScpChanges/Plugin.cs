using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.ScpChanges
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: SCP Changes";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.SC";
        public override Version Version { get; } = new Version(3, 3, 1);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 2);
        public ScpChangesEventHandlers ScpChangesEventHandlers;
        
        public override void OnEnabled()
        {
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP SC: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }

            ScpChangesEventHandlers = new ScpChangesEventHandlers(this);
            Player.UsedItem += ScpChangesEventHandlers.OnUsingItem;
            Player.ChangingRole += ScpChangesEventHandlers.OnChangingRole;
            Player.Hurting += ScpChangesEventHandlers.OnHurting;
            Instance = this;
            Base.Plugin.Instance.VvupSc = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupSc = false;
            Player.UsedItem -= ScpChangesEventHandlers.OnUsingItem;
            Player.ChangingRole -= ScpChangesEventHandlers.OnChangingRole;
            Player.Hurting -= ScpChangesEventHandlers.OnHurting;
            ScpChangesEventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}