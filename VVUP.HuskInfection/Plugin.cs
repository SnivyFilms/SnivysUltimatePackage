using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;
using VVUP.CustomRoles.API;
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
        public override Version Version { get; } = new Version(3, 2, 1);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 2);

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
            CustomAbility.RegisterAbilities(false, null);
            Config.CustomRoleConfig.HuskZombies.Register();
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

                    if (!CustomRoles.Plugin.Instance.Roles.ContainsKey(team))
                        CustomRoles.Plugin.Instance.Roles.Add(team, new());

                    for (int i = 0; i < role.SpawnProperties.Limit; i++)
                        CustomRoles.Plugin.Instance.Roles[team].Add(custom);
                    Log.Debug($"Roles {team} now has {CustomRoles.Plugin.Instance.Roles[team].Count} elements.");
                }
            }

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