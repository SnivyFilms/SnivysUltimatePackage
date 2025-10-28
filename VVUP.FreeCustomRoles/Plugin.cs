using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;
using VVUP.CustomRoles.API;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.FreeCustomRoles
{
    public class Plugin : Plugin<MasterConfig>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Lower;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Free Custom Roles";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.FCR";
        public override Version Version { get; } = new Version(3, 4, 2);
        public override Version RequiredExiledVersion { get; } = new Version(9, 10, 0);

        public SsssEventHandlers SsssEventHandlers;
        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP FCR: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.CR"))
            {
                Log.Error("VVUP FCR: Custom Roles Module is not present, disabling module");
                base.OnDisabled();
                return;
            }
            HashSet<CustomRole> existingRoles = new HashSet<CustomRole>(CustomRole.Registered);
            Config.FreeCustomRoles1.Register();
            Config.FreeCustomRoles2.Register();
            Config.FreeCustomRoles3.Register();
            Config.FreeCustomRoles4.Register();
            Config.FreeCustomRoles5.Register();
            Config.FreeCustomRoles6.Register();
            Config.FreeCustomRoles7.Register();
            Config.FreeCustomRoles8.Register();
            Config.FreeCustomRoles9.Register();
            Config.FreeCustomRoles10.Register();
            Config.FreeCustomRoles11.Register();
            Config.FreeCustomRoles12.Register();
            Config.FreeCustomRoles13.Register();
            Config.FreeCustomRoles14.Register();
            Config.FreeCustomRoles15.Register();
            Config.FreeCustomRoles16.Register();
            Config.FreeCustomRoles17.Register();
            Config.FreeCustomRoles18.Register();
            Config.FreeCustomRoles19.Register();
            Config.FreeCustomRoles20.Register();
            
            foreach (CustomRole role in CustomRole.Registered)
            {
                if (!existingRoles.Contains(role) && role is ICustomRole custom)
                {
                    Log.Debug($"VVUP FCR: Adding {role.Name} to dictionary..");
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
                    Log.Debug($"VVUP FCR: Roles {team} now has {CustomRoles.Plugin.Instance.Roles[team].Count} elements.");
                }
            }
            Base.Plugin.Instance.VvupFcr = true;
            existingRoles.Clear();
            SsssEventHandlers = new SsssEventHandlers(this);
            Player.Verified += SsssEventHandlers.OnVerified;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomRole.UnregisterRoles();
            Player.Verified -= SsssEventHandlers.OnVerified;
            SsssEventHandlers = null;
            Base.Plugin.Instance.VvupFcr = false;
            Instance = null;
            base.OnDisabled();
        }
    }
}