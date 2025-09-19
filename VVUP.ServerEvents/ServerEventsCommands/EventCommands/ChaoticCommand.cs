using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsConfigs;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class ChaoticCommand : ICommand
    {
        public string Command { get; set; } = "Chaotic";
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = "Starts the Chaotic Event";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.rund"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            ChaoticEventHandlers chaoticHandlers = new ChaoticEventHandlers();
            response = "Starting Chaotic Event";
            Log.Debug($"{sender} has started the Chaotic Event");
            return true;
        }
    }
}