using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsConfigs;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class PeanutInfectionCommand : ICommand
    {
        public string Command { get; set; } = "173Infection";
        public string[] Aliases { get; set; } = { "PeanutInfection", "Infection" };
        public string Description { get; set; } = "Starts the 173 Infection";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.runn"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            
            PeanutInfectionEventHandlers infectionEventHandlers = new PeanutInfectionEventHandlers();
            response = "Starting Peanut Infection Event";
            Log.Debug($"{sender} has started the Peanut Infection Event");
            return true;
        }
    }
}