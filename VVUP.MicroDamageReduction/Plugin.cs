using System;
using System.Reflection;
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
        public override string Prefix { get; } = "VVUP.MDR";
        public override string Author =>
            Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyCompanyAttribute>()?
                .Company ?? "Unknown";
        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version;
        public override Version RequiredExiledVersion =>
            Version.Parse(
                Assembly.GetExecutingAssembly()
                    .GetCustomAttributes<AssemblyMetadataAttribute>()
                    .First(x => x.Key == "RequiredExiledVersion")
                    .Value);
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