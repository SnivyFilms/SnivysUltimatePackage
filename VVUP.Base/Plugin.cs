using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using VVUP.Base.EventHandlers;
using Map = Exiled.Events.Handlers.Map;
using Server = Exiled.Events.Handlers.Server;

namespace VVUP.Base
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Base";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.Base";
        public override Version Version { get; } = new Version(3, 5, 6);
        public override Version RequiredExiledVersion { get; } = new Version(9, 12, 2);

        public bool VvupCi = false; // Custom Items
        public bool VvupCr = false; // Custom Roles
        public bool VvupFcr = false; // Free Custom Roles
        public bool VvupSe = false; // Server Events
        public bool VvupMdr = false; // Micro Damage Reduction
        public bool VvupWe = false; // Weapon Evaporate
        public bool VvupRs = false; // Round Start
        public bool VvupSc = false; // SCP Changes
        public bool VvupFa = false; // Flamingo Adjustments
        public bool VvupHk = false; // Husk Infection
        public bool VvupVo = false; // Votes
        public bool VvupCt = false; // Credit Tags
        public bool VvupCe = false; // Custom Escapes
        public bool VvupPne = false; // Post Nuke Enhancements
        
        public CustomItemEventHandlers CustomItemEventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            CustomItemEventHandlers = new CustomItemEventHandlers(this);
            Server.RoundStarted += CustomItemEventHandlers.OnRoundStarted;
            Server.WaitingForPlayers += CustomItemEventHandlers.OnWaitingForPlayers;
            Map.PickupAdded += CustomItemEventHandlers.AddGlow;
            Map.PickupDestroyed += CustomItemEventHandlers.RemoveGlow;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            VvupCi = false;
            VvupCr = false;
            VvupFcr = false;
            VvupSe = false;
            VvupMdr = false;
            VvupWe = false;
            VvupRs = false;
            VvupSc = false;
            VvupFa = false;
            VvupHk = false;
            VvupVo = false;
            VvupCt = false;
            VvupCe = false;
            VvupPne = false;
            
            Server.RoundStarted -= CustomItemEventHandlers.OnRoundStarted;
            Server.WaitingForPlayers -= CustomItemEventHandlers.OnWaitingForPlayers;
            Map.PickupAdded -= CustomItemEventHandlers.AddGlow;
            Map.PickupDestroyed -= CustomItemEventHandlers.RemoveGlow;
            CustomItemEventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}