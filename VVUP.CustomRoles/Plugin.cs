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
using VVUP.CustomRoles.Configs;
using VVUP.CustomRoles.EventHandlers;
using Config = VVUP.CustomRoles.Configs.Config;
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
        public override Version Version { get; } = new Version(3, 5, 2);
        public override Version RequiredExiledVersion { get; } = new Version(9, 10, 2);
        
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
            
            HashSet<CustomRole> existingRoles = new HashSet<CustomRole>(CustomRole.Registered);
            HashSet<CustomAbility> existingAbilities = new HashSet<CustomAbility>(CustomAbility.Registered);
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
            Config.CustomRolesConfig.ClassDDruggies.Register();
            Config.CustomRolesConfig.TeslaTechnicians.Register();

            foreach (CustomRole role in CustomRole.Registered)
            {
                if (role.CustomAbilities is not null)
                {
                    foreach (var ability in role.CustomAbilities.Where(ability => !existingAbilities.Contains(ability)))
                    {
                        Log.Debug($"VVUP CR: Registering ability {ability.Name}");
                        ability.Register();
                    }
                }
                if (!existingRoles.Contains(role) && role is ICustomRole custom)
                {
                    Log.Debug($"VVUP CR: Adding {role.Name} to dictionary..");
                    StartTeam team = custom.StartTeam switch
                    {
                        var t when t.HasFlag(StartTeam.Chaos) => StartTeam.Chaos,
                        var t when t.HasFlag(StartTeam.Guard) => StartTeam.Guard,
                        var t when t.HasFlag(StartTeam.Ntf) => StartTeam.Ntf,
                        var t when t.HasFlag(StartTeam.Scientist) => StartTeam.Scientist,
                        var t when t.HasFlag(StartTeam.ClassD) => StartTeam.ClassD,
                        var t when t.HasFlag(StartTeam.Scp) => StartTeam.Scp,
                        var t when t.HasFlag(StartTeam.Scp049) => StartTeam.Scp049,
                        var t when t.HasFlag(StartTeam.Scp079) => StartTeam.Scp079,
                        var t when t.HasFlag(StartTeam.Scp096) => StartTeam.Scp096,
                        var t when t.HasFlag(StartTeam.Scp106) => StartTeam.Scp106,
                        var t when t.HasFlag(StartTeam.Scp173) => StartTeam.Scp173,
                        var t when t.HasFlag(StartTeam.Scp939) => StartTeam.Scp939,
                        var t when t.HasFlag(StartTeam.Scp3114) => StartTeam.Scp3114,
                        _ => StartTeam.Other
                    };

                    if (!Roles.ContainsKey(team))
                        Roles.Add(team, new());

                    for (int i = 0; i < role.SpawnProperties.Limit; i++)
                        Roles[team].Add(custom);
                    Log.Debug($"VVUP CR: Roles {team} now has {Roles[team].Count} elements.");
                }
            }
            // I was trying something with external custom roles, no cigar for now, maybe ill come back in the future to try it again
            /*if (Config.CustomRoleWrapperConfig.EnableCustomRoleCompatibility)
            {
                Log.Debug("Registering custom roles from wrapper configuration...");

                HashSet<uint> registeredRoleIds = new HashSet<uint>(
                    CustomRole.Registered
                        .Where(r => r is ICustomRole)
                        .Select(r => r.Id));
    
                foreach (var wrapper in Config.CustomRoleWrapperConfig.CustomRoles)
                {
                    if (registeredRoleIds.Contains(wrapper.CustomRoleId))
                        continue;
    
                    StartTeam team = wrapper.StartTeam;

                    if (!Roles.ContainsKey(team))
                        Roles.Add(team, new());

                    var customRoleAdapter = new CustomRoleWrapperAdapter(wrapper);
        
                    for (int i = 0; i < wrapper.Limit; i++)
                        Roles[team].Add(customRoleAdapter);
        
                    Log.Debug($"Registered external custom role ID {wrapper.CustomRoleId} with team {team} and chance {wrapper.Chance}");
                    Log.Debug($"VVUP CR: Roles {team} now has {Roles[team].Count} elements.");
                }
            }*/

            existingRoles.Clear();
            existingAbilities.Clear();
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