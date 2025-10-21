using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;

namespace VVUP.Votes
{
    public class Plugin : Plugin<VoteConfig>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Votes";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.V";
        public override Version Version { get; } = new Version(3, 3, 2);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 2);

        public override void OnEnabled()
        {
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP V: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }

            Instance = this;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();
        }
    }
}