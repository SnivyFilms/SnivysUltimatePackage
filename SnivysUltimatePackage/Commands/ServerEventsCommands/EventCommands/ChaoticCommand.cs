﻿using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using SnivysUltimatePackage.Configs.ServerEventsConfigs;
using SnivysUltimatePackage.EventHandlers.ServerEventsEventHandlers;

namespace SnivysUltimatePackage.Commands.ServerEventsCommands.EventCommands
{
    internal class ChaoticCommand : ICommand
    {
        public string Command { get; set; } = "Chaotic";
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = "Starts the Chaotic Event";
        private static ServerEventsMasterConfig _config = new();
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!_config.IsEnabled)
            {
                response = "The custom events part of this plugin is disabled.";
                return false;
            }
            
            if (!sender.CheckPermission("vvevents.run.disruptive"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            if (OperationCrossfireEventHandlers.OcfStarted)
            {
               response =
                   "Operation Crossfire is running, this event is not allowed to be ran at the same time as Operation Crossfire";
               return false;
            }
            
            ChaoticEventHandlers chaoticHandlers = new ChaoticEventHandlers();
            response = "Starting Chaotic Event";
            Log.Debug($"{sender} has started the Chaotic Event");
            return true;
        }
    }
}