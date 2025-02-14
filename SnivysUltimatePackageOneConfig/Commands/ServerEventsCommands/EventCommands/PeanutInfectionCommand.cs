﻿using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using SnivysUltimatePackageOneConfig.Configs.ServerEventsConfigs;
using SnivysUltimatePackageOneConfig.EventHandlers.ServerEventsEventHandlers;

namespace SnivysUltimatePackageOneConfig.Commands.ServerEventsCommands.EventCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class PeanutInfectionCommand : ICommand
    {
        public string Command { get; set; } = "173Infection";
        public string[] Aliases { get; set; } = { "PeanutInfection", "Infection" };
        public string Description { get; set; } = "Starts the 173 Infection";
        private static ServerEventsMasterConfig _config = new();
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!_config.IsEnabled)
            {
                response = "The custom events part of this plugin is disabled.";
                return false;
            }
            
            if (!sender.CheckPermission("vvevents.run"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            PeanutInfectionEventHandlers infectionEventHandlers = new PeanutInfectionEventHandlers();
            response = "Starting Peanut Infection Event";
            Log.Debug($"{sender} has started the Peanut Infection Event");
            return true;
        }
    }
}