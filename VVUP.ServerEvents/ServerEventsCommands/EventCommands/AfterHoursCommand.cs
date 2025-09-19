using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsConfigs;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    public class AfterHoursCommand : ICommand
    {
        public string Command { get; set; } = "AfterHours";
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = "Starts the AfterHours Event";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.runn"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            AfterHoursEventHandlers afterHours = new AfterHoursEventHandlers();
            response = "Starting After Hours Event";
            Log.Debug($"{sender} has started the After Hours Event");
            return true;
        }
    }
}