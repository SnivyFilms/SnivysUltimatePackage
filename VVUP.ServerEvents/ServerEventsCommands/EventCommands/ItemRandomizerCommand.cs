using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    public class ItemRandomizerCommand : ICommand
    {
        public string Command { get; } = "ItemRandomizer";
        public string[] Aliases { get; } = { "Randomizer", "ItemRandom", "IR" };
        public string Description { get; } = "Starts item randomizer event";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.rund"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            ItemRandomizerEventHandlers itemRandomizerEventHandlers = new ItemRandomizerEventHandlers();
            response = "Starting Item Randomizer Event";
            Log.Debug($"{sender} has started the Item Randomizer Event");
            return true;
        }
    }
}