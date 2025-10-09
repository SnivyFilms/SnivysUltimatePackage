using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    public class NoSpectatingPlayersCommand : ICommand
    {
        public string Command { get; set; } = "NoSpectatingPlayers";
        public string[] Aliases { get; set; } = {"NoSpectating", "NoSpecs", "NS"};
        public string Description { get; set; } = "Disables spectating to all players";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("vvevents.runn"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }
            NoSpectatingPlayersEventHandlers noSpectatingHandler = new NoSpectatingPlayersEventHandlers();
            response = "Starting No Spectating Players Event";
            Log.Debug($"{sender} has started the No Spectators Event");
            return true;
        }
    }
}