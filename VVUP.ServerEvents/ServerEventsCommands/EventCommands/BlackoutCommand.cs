using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsConfigs;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class BlackoutCommand : ICommand
    {
        public string Command { get; set; } = "Blackout";
        public string[] Aliases { get; set; } = { "LightsOut" };
        public string Description { get; set; } = "Starts the Blackout Event";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.runn"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            BlackoutEventHandlers blackoutEventHandlers = new BlackoutEventHandlers();
            response = "Starting Blackout Event";
            Log.Debug($"{sender} has started the Blackout Event");
            return true;
        }
    }
}