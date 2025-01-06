﻿using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using SnivysUltimatePackage.API;
using SnivysUltimatePackage.Configs;
using SnivysUltimatePackage.EventHandlers;
using SnivysUltimatePackage.EventHandlers.Custom;
using SnivysUltimatePackage.EventHandlers.ServerEventsEventHandlers;
using UserSettings.ServerSpecific;
using Player = Exiled.Events.Handlers.Player;
using Scp049Events = Exiled.Events.Handlers.Scp049;
using Server = Exiled.Events.Handlers.Server;

namespace SnivysUltimatePackage
{
    public class Plugin : Plugin<MasterConfig>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "Snivy's Ultimate Plugin Package";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUltimatePluginPackage";
        public override Version Version { get; } = new Version(1, 9, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 2, 2);
        public static int ActiveEvent = 0;
        
        public Dictionary<StartTeam, List<ICustomRole>> Roles { get; } = new();
        
        public CustomRoleEventHandler CustomRoleEventHandler;
        public ServerEventsMainEventHandler ServerEventsMainEventHandler;
        public MicroDamageReductionEventHandler MicroDamageReductionEventHandler;
        public MicroEvaporateEventHandlers MicroEvaporateEventHandlers;
        public FlamingoAdjustmentEventHandlers FlamingoAdjustmentEventHandlers;
        public EscapeDoorOpenerEventHandlers EscapeDoorOpenerEventHandlers;
        public Scp1576SpectatorViewerEventHandlers Scp1576SpectatorViewerEventHandlers;
        public SsssEventHandler SsssEventHandler;

        public override void OnEnabled()
        {
            Instance = this;

            //Custom Items
            if (Instance.Config.CustomItemsConfig.IsEnabled)
                CustomItem.RegisterItems(overrideClass: Instance.Config.CustomItemsConfig);
            
            //Custom Roles
            if (Instance.Config.CustomRolesConfig.IsEnabled)
            {
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
            }

            // Custom Abilities
            if (Instance.Config.CustomRolesAbilitiesConfig.IsEnabled)
                CustomAbility.RegisterAbilities();
                
            // Server Events
            ServerEventsMainEventHandler = new ServerEventsMainEventHandler(this);
            Server.RoundStarted += ServerEventsMainEventHandler.OnRoundStart;
            Server.RoundEnded += ServerEventsMainEventHandler.OnEndingRound;
            Server.WaitingForPlayers += ServerEventsMainEventHandler.OnWaitingForPlayers;
                
            //Micro Damage Reduction
            MicroDamageReductionEventHandler = new MicroDamageReductionEventHandler(this);
            Player.Hurting += MicroDamageReductionEventHandler.OnPlayerHurting;
                
            //Micro Evaporate
            MicroEvaporateEventHandlers = new MicroEvaporateEventHandlers(this);
            Player.Dying += MicroEvaporateEventHandlers.OnDying;
                
            //Flamingo Adjustment
            FlamingoAdjustmentEventHandlers = new FlamingoAdjustmentEventHandlers(this);
            Player.Hurting += FlamingoAdjustmentEventHandlers.OnHurting;
                
            //Escape Door Opener
            EscapeDoorOpenerEventHandlers = new EscapeDoorOpenerEventHandlers(this);
            Server.RoundStarted += EscapeDoorOpenerEventHandlers.OnRoundStarted;
                
            //SCP 1576 Spectator Viewer
            Scp1576SpectatorViewerEventHandlers = new Scp1576SpectatorViewerEventHandlers(this);
            Player.UsedItem += Scp1576SpectatorViewerEventHandlers.OnUsingItem;
            
            //SSSS
            SsssEventHandler = new SsssEventHandler(this);
            Player.ChangingRole += SsssEventHandler.OnChangingRole;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += SsssEventHandler.OnSettingValueReceived;
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomItem.UnregisterItems();
            CustomRole.UnregisterRoles();
            CustomAbility.UnregisterAbilities();

            //Custom Roles Event Handlers
            Server.RoundStarted -= CustomRoleEventHandler.OnRoundStarted; 
            Server.RespawningTeam -= CustomRoleEventHandler.OnRespawningTeam;
            Scp049Events.FinishingRecall -= CustomRoleEventHandler.FinishingRecall;
            CustomRoleEventHandler = null;
            
            //Server Events Event Handlers
            Server.RoundEnded -= ServerEventsMainEventHandler.OnEndingRound;
            Server.WaitingForPlayers -= ServerEventsMainEventHandler.OnWaitingForPlayers;
            ServerEventsMainEventHandler = null;
            
            //Micro Damage Reduction Event Handler
            Player.Hurting -= MicroDamageReductionEventHandler.OnPlayerHurting;
            MicroDamageReductionEventHandler = null;
            
            //Micro Evaporate Players Event Handler
            Player.Dying -= MicroEvaporateEventHandlers.OnDying;
            MicroEvaporateEventHandlers = null;
            
            //Flamingo Adjustment Event Handler
            Player.Hurting -= FlamingoAdjustmentEventHandlers.OnHurting;
            FlamingoAdjustmentEventHandlers = null;
            
            //Escape Door Opener Event Handler
            Server.RoundStarted -= EscapeDoorOpenerEventHandlers.OnRoundStarted;
            EscapeDoorOpenerEventHandlers = null;

            //SCP 1576 Spectator Viewer Event Handler
            Player.UsedItem -= Scp1576SpectatorViewerEventHandlers.OnUsingItem;
            Scp1576SpectatorViewerEventHandlers = null;

            //SSSS
            Player.ChangingRole -= SsssEventHandler.OnChangingRole;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= SsssEventHandler.OnSettingValueReceived;
            SsssEventHandler = null;
            
            Instance = null;
            base.OnDisabled();
        }
    }
}