using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class FreezingTemperaturesCommand : ICommand
    {
        public string Command { get; set; } = "FreezingTemps";
        public string[] Aliases { get; set; } = { "Cold", "Freezing" };
        public string Description { get; set; } = "Starts the Freezing Temperature Event";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        { 
            if (!sender.CheckPermission("vvevents.rund"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            FreezingTemperaturesEventHandlers freezingTemperaturesHandlers = new FreezingTemperaturesEventHandlers();
            response = "Starting Freezing Temperature Event";
            Log.Debug($"{sender} has started the Freezing Temperatures Event");
            return true;
        }
    }
}