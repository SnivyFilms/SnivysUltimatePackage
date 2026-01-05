using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using VVUP.ServerEvents.ServerEventsEventHandlers;

namespace VVUP.ServerEvents.ServerEventsCommands.EventCommands
{
    internal class SnowballsVsScpsCommand : ICommand
    {
        public string Command { get; set; } = "SnowballsVsScps";
        public string[] Aliases { get; set; } = { "SvS", "SnowballFight" };
        public string Description { get; set; } = "Starts the Snowballs Vs Scps Event";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            //response = "I am sorry, the snowballs has been removed from the game, this event cannot run";
            //return false;
            
            if (!sender.CheckPermission("vvevents.rund"))
            {
                response = "You do not have the required permission to use this command";
                return false;
            }

            SnowballsVsScpsEventHandlers snowballsVsScpsEventHandlers = new SnowballsVsScpsEventHandlers();
            response = "Starting Snowballs Vs Scps Event";
            Log.Debug($"{sender} has started Snowballs Vs Scps Event");
            return true;
        }
    }
}