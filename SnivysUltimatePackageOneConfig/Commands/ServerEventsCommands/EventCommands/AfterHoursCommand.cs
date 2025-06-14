using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using SnivysUltimatePackageOneConfig.Configs.ServerEventsConfigs;
using SnivysUltimatePackageOneConfig.EventHandlers.ServerEventsEventHandlers;

namespace SnivysUltimatePackageOneConfig.Commands.ServerEventsCommands.EventCommands
{
    public class AfterHoursCommand : ICommand
    {
        public string Command { get; set; } = "AfterHours";
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = "Starts the AfterHours Event";
        private static ServerEventsMasterConfig _config = new();
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!_config.IsEnabled)
            {
                response = "The custom events part of this plugin is disabled.";
                return false;
            }
            
            if (!sender.CheckPermission("vvevents.runn"))
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
            
            AfterHoursEventHandlers afterHours = new AfterHoursEventHandlers();
            response = "Starting After Hours Event";
            Log.Debug($"{sender} has started the After Hours Event");
            return true;
        }
    }
}