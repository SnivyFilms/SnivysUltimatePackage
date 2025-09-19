using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsConfigs;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class GravityCommand : ICommand
    {
        public string Command { get; set; } = "LowGravity";
        public string[] Aliases { get; set; } = { "Moon", "LG" };
        public string Description { get; set; } = "Starts the Low Gravity Event";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.runn"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            GravityEventHandlers gravityEventHandlers = new GravityEventHandlers();
            response = "Starting Low Gravity Event";
            Log.Debug($"{sender} has started the Low Gravity Event");
            return true;
        }
    }
}