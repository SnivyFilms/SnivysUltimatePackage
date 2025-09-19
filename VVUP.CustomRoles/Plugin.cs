using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;
using UserSettings.ServerSpecific;
using VVUP.CustomRoles.API;
using VVUP.CustomRoles.EventHandlers;
using Server = Exiled.Events.Handlers.Server;
using Scp049Events = Exiled.Events.Handlers.Scp049;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CustomRoles
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Custom Roles";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.CR";
        public override Version Version { get; } = new Version(3, 2, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 2);
        
        public Dictionary<StartTeam, List<ICustomRole>> Roles { get; } = new();
        public CustomRoleEventHandler CustomRoleEventHandler;
        public SsssEventHandlers SsssEventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP CR: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }

            CustomAbility.RegisterAbilities(false, null);
            
            CustomRoleEventHandler = new CustomRoleEventHandler(this);
            Config.CustomRolesConfig.ContainmentScientists.Register();
            Config.CustomRolesConfig.LightGuards.Register();
            Config.CustomRolesConfig.Biochemists.Register();
            Config.CustomRolesConfig.ContainmentGuards.Register();
            Config.CustomRolesConfig.BorderPatrols.Register();
            Config.CustomRolesConfig.Nightfalls.Register();
            Config.CustomRolesConfig.A7Chaoss.Register();
            Config.CustomRolesConfig.Flippeds.Register();
            Config.CustomRolesConfig.TelepathicChaos.Register();
            Config.CustomRolesConfig.JuggernautChaos.Register();
            Config.CustomRolesConfig.CISpies.Register();
            Config.CustomRolesConfig.MtfWisps.Register();
            Config.CustomRolesConfig.DwarfZombies.Register();
            Config.CustomRolesConfig.ExplosiveZombies.Register();
            Config.CustomRolesConfig.CiPhantoms.Register();
            Config.CustomRolesConfig.MedicZombies.Register();
            Config.CustomRolesConfig.LockpickingClassDs.Register();
            Config.CustomRolesConfig.Demolitionists.Register();
            Config.CustomRolesConfig.Vanguards.Register();
            Config.CustomRolesConfig.TheoreticalPhysicistScientists.Register();
            Config.CustomRolesConfig.MtfParamedics.Register();
            Config.CustomRolesConfig.ClassDAnalysts.Register();
            Config.CustomRolesConfig.ClassDTanks.Register();
            Config.CustomRolesConfig.InfectedZombies.Register();
            Config.CustomRolesConfig.PoisonousZombies.Register();
            Config.CustomRolesConfig.SpeedsterZombies.Register();
            Config.CustomRolesConfig.TeleportZombies.Register();
            Config.CustomRolesConfig.SoundBreaker173.Register();
            Config.CustomRolesConfig.Replicant.Register();
            Config.CustomRolesConfig.Dwarfs.Register();
            Config.CustomRolesConfig.PaperMen.Register();
            Config.CustomRolesConfig.QuantumGravityResearchers.Register();

            foreach (CustomRole role in CustomRole.Registered)
            {
                if (role is ICustomRole custom)
                {
                    Log.Debug($"Adding {role.Name} to dictionary..");
                    StartTeam team;
                    if (custom.StartTeam.HasFlag(StartTeam.Chaos))
                        team = StartTeam.Chaos;
                    else if (custom.StartTeam.HasFlag(StartTeam.Guard))
                        team = StartTeam.Guard;
                    else if (custom.StartTeam.HasFlag(StartTeam.Ntf))
                        team = StartTeam.Ntf;
                    else if (custom.StartTeam.HasFlag(StartTeam.Scientist))
                        team = StartTeam.Scientist;
                    else if (custom.StartTeam.HasFlag(StartTeam.ClassD))
                        team = StartTeam.ClassD;
                    else if (custom.StartTeam.HasFlag(StartTeam.Scp))
                        team = StartTeam.Scp;
                    else
                        team = StartTeam.Other;

                    if (!Roles.ContainsKey(team))
                        Roles.Add(team, new());

                    for (int i = 0; i < role.SpawnProperties.Limit; i++)
                        Roles[team].Add(custom);
                    Log.Debug($"Roles {team} now has {Roles[team].Count} elements.");
                }
            }
            
            Server.RoundStarted += CustomRoleEventHandler.OnRoundStarted;
            Server.RespawningTeam += CustomRoleEventHandler.OnRespawningTeam;
            Scp049Events.FinishingRecall += CustomRoleEventHandler.FinishingRecall;
            SsssEventHandlers = new SsssEventHandlers(this);
            Player.Verified += SsssEventHandlers.OnVerified;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += SsssEventHandlers.OnSettingValueReceived;
            Base.Plugin.Instance.VvupCr = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomRole.UnregisterRoles();
            Server.RoundStarted -= CustomRoleEventHandler.OnRoundStarted;
            Server.RespawningTeam -= CustomRoleEventHandler.OnRespawningTeam;
            Scp049Events.FinishingRecall -= CustomRoleEventHandler.FinishingRecall;
            Base.Plugin.Instance.VvupCr = false;
            Player.Verified -= SsssEventHandlers.OnVerified;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= SsssEventHandlers.OnSettingValueReceived;
            SsssEventHandlers = null;
            CustomRoleEventHandler = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}