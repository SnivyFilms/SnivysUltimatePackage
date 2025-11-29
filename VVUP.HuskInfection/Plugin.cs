using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;
using VVUP.CustomRoles.API;
using VVUP.HuskInfection.EventHandlers;
using Config = VVUP.HuskInfection.Configs.Config;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace VVUP.HuskInfection
{
    public class Plugin : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Lower;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Husk Infection";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.HK";
        public override Version Version { get; } = new Version(3, 5, 1);
        public override Version RequiredExiledVersion { get; } = new Version(9, 10, 2);

        public HuskInfectionEventHandlers HuskInfectionEventHandlers;
        public SsssEventHandlers SsssEventHandlers;
        
        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP HK: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }

            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.CR"))
            {
                Log.Error("VVUP HK: Custom Roles Module is not present, disabling module");
                base.OnDisabled();
                return;
            }
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.CI"))
            {
                Log.Error("VVUP HK: Custom Items Module is not present, disabling module");
                base.OnDisabled();
                return;
            }
            HashSet<CustomRole> existingRoles = new HashSet<CustomRole>(CustomRole.Registered);
            HashSet<CustomAbility> existingAbilities = new HashSet<CustomAbility>(CustomAbility.Registered);
            Config.CustomRoleConfig.HuskZombies.Register();
            foreach (CustomRole role in CustomRole.Registered)
            {
                if (role.CustomAbilities is not null)
                {
                    foreach (var ability in role.CustomAbilities.Where(ability => !existingAbilities.Contains(ability)))
                    {
                        Log.Debug($"VVUP HK: Registering ability {ability.Name}");
                        ability.Register();
                    }
                }
                if (!existingRoles.Contains(role) && role is ICustomRole custom)
                {
                    Log.Debug($"VVUP HK: Adding {role.Name} to dictionary..");
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

                    if (!CustomRoles.Plugin.Instance.Roles.ContainsKey(team))
                        CustomRoles.Plugin.Instance.Roles.Add(team, new());

                    for (int i = 0; i < role.SpawnProperties.Limit; i++)
                        CustomRoles.Plugin.Instance.Roles[team].Add(custom);
                    Log.Debug($"VVUP HK: Roles {team} now has {CustomRoles.Plugin.Instance.Roles[team].Count} elements.");
                }
            }
            existingRoles.Clear();
            existingAbilities.Clear();
            CustomItem.RegisterItems(overrideClass: Instance.Config.CustomItemConfig);
            HuskInfectionEventHandlers = new HuskInfectionEventHandlers(this);
            Server.WaitingForPlayers += HuskInfectionEventHandlers.OnWaitingForPlayers;
            Server.RoundEnded += HuskInfectionEventHandlers.OnRoundEnded;
            Player.VoiceChatting += HuskInfectionEventHandlers.OnVoiceChatting;
            Player.ChangingRole += HuskInfectionEventHandlers.OnRoleChange;
            SsssEventHandlers = new SsssEventHandlers(this);
            Player.Verified += SsssEventHandlers.OnVerified;
            Base.Plugin.Instance.VvupHk = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomRole.UnregisterRoles();
            CustomItem.UnregisterItems();
            CustomAbility.UnregisterAbilities();
            Server.WaitingForPlayers -= HuskInfectionEventHandlers.OnWaitingForPlayers;
            Server.RoundEnded -= HuskInfectionEventHandlers.OnRoundEnded;
            Player.VoiceChatting -= HuskInfectionEventHandlers.OnVoiceChatting;
            Player.ChangingRole -= HuskInfectionEventHandlers.OnRoleChange;
            Player.Verified -= SsssEventHandlers.OnVerified;
            SsssEventHandlers = null;
            HuskInfectionEventHandlers = null;
            Base.Plugin.Instance.VvupHk = false;
            Instance = null;
            base.OnDisabled();
        }
    }
}