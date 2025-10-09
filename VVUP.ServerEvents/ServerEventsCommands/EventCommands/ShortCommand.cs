using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class ShortCommand : ICommand
    {
        public string Command { get; set; } = "ShortPeople";
        public string[] Aliases { get; set; } = { "Dwarf", "Tiny" };
        public string Description { get; set; } = "Starts the Short People Event";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.rund"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            ShortEventHandlers shortEventHandlers = new ShortEventHandlers();
            response = "Starting Short People Event";
            Log.Debug($"{sender} has started the Short People Event");
            return true;
        }
    }
}