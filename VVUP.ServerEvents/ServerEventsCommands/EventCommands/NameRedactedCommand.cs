using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsConfigs;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class NameRedactedCommand : ICommand
    {
        public string Command { get; set; } = "NameRedacted";
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = "Removes player's nicknames and sets them to something else";
        private static ServerEventsMasterConfig _config = new();
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.runn"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            NameRedactedEventHandlers nameRedactedHandler = new NameRedactedEventHandlers();
            response = "Starting Name Redacted Event";
            Log.Debug($"{sender} has started the Name Redacted Event");
            return true;
        }
    }
}