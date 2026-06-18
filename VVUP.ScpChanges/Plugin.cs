using System.Reflection;
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
        public override string Author { get; } = typeof(Plugin).Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company
            ?? throw new InvalidOperationException("Missing assembly company metadata.");
        public override string Prefix { get; } = "VVUP.SC";
        public override Version Version => GetType().Assembly.GetName().Version;
        public override Version RequiredExiledVersion { get; } = global::System.Version.Parse(
            typeof(Plugin).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().First(a => a.Key == "RequiredExiledVersion").Value);
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
            Player.Spawned += ScpChangesEventHandlers.OnChangingRole;
            Player.Hurting += ScpChangesEventHandlers.OnHurting;
            
            // TODO: reimplement
            // Exiled.Events.Handlers.Warhead.Starting += ScpChangesEventHandlers.OnNukeStarted;
            Instance = this;
            Base.Plugin.Instance.VvupSc = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupSc = false;
            Player.UsedItem -= ScpChangesEventHandlers.OnUsingItem;
            Player.Spawned -= ScpChangesEventHandlers.OnChangingRole;
            Player.Hurting -= ScpChangesEventHandlers.OnHurting;
            
            // TODO: reimplement
            // Exiled.Events.Handlers.Warhead.Starting -= ScpChangesEventHandlers.OnNukeStarted;
            ScpChangesEventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}