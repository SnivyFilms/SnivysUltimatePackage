using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.MicroDamageReduction
{
    public class Plugin : Plugin<MicroDamageReductionConfig>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Micro Damage Reduction";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.MDR";
        public override Version Version { get; } = new Version(3, 3, 3);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 2);
        public MicroDamageReductionEventHandler MicroDamageReductionEventHandler;
        
        public override void OnEnabled()
        {
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP MDR: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }

            MicroDamageReductionEventHandler = new MicroDamageReductionEventHandler(this);
            Player.Hurting += MicroDamageReductionEventHandler.OnPlayerHurting;
            Instance = this;
            Base.Plugin.Instance.VvupMdr = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupMdr = false;
            Player.Hurting -= MicroDamageReductionEventHandler.OnPlayerHurting;
            MicroDamageReductionEventHandler = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}