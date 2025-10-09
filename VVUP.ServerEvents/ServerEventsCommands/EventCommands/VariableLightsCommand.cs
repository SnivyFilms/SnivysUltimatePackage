using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class VariableLightCommand : ICommand
    {
        public string Command { get; set; } = "VariableLights";
        public string[] Aliases { get; set; } = { "RandomLights", "ColorfulLights" };
        public string Description { get; set; } = "Starts the Variable Lights Event. (PHOTOSENSITIVITY WARNING!)";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.runn"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            VariableLightsEventHandlers variableEventHandlers = new VariableLightsEventHandlers();
            response = "Starting Variable Lights Event.";
            Log.Debug($"{sender} has started the Variable Lights Event");
            return true;
        }
    }
}