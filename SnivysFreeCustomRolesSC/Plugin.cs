﻿using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;
using SnivysUltimatePackage.API;
using Server = Exiled.Events.Handlers.Server;

namespace SnivysFreeCustomRolesSC
{
    public class Plugin : Plugin<MasterConfig>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "Snivy's Free Custom Roles (For Snivy's Ultimate Package Split Config)";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVFreeCustomRoles";
        public override Version Version { get; } = new Version(1, 1, 2);
        public override Version RequiredExiledVersion { get; } = new Version(9, 6, 1);
        
        public ReloadConfigsEventHandler ReloadConfigsEventHandler;

        public override void OnEnabled()
        {
            Instance = this;
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUltimatePluginPackage"))
            {
                Log.Error("VVUltimatePluginFreeCustomRolesSC: VVUltimatePluginPackage is missing, disabling plugin.");
                base.OnDisabled();
                return;
            }

            Config.LoadConfigs();
            
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

                    if (!SnivysUltimatePackage.Plugin.Instance.Roles.ContainsKey(team))
                        SnivysUltimatePackage.Plugin.Instance.Roles.Add(team, new());

                    for (int i = 0; i < role.SpawnProperties.Limit; i++)
                        SnivysUltimatePackage.Plugin.Instance.Roles[team].Add(custom);
                    Log.Debug($"Roles {team} now has {SnivysUltimatePackage.Plugin.Instance.Roles[team].Count} elements.");
                }
            }
            
            ReloadConfigsEventHandler = new ReloadConfigsEventHandler(this);
            Server.ReloadedConfigs += ReloadConfigsEventHandler.OnReloadingConfigs;
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomRole.UnregisterRoles();
            Server.ReloadedConfigs -= ReloadConfigsEventHandler.OnReloadingConfigs;
            ReloadConfigsEventHandler = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}