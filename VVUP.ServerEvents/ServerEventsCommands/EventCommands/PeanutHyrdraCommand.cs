using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class PeanutHydraCommand : ICommand
    {
        public string Command { get; set; } = "173Hydra";
        public string[] Aliases { get; set; } = { "PeanutHydra", "Hydra" };
        public string Description { get; set; } = "Starts the 173 Hydra";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.rund"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            PeanutHydraEventHandlers hydraEventHandlers = new PeanutHydraEventHandlers();
            response = "Starting Peanut Hydra Event";
            Log.Debug($"{sender} has started the Peanut Hydra Event");
            return true;
        }
    }
}