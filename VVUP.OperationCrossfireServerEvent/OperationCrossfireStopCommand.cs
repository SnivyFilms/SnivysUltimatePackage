﻿using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;

namespace VVUP.OperationCrossfireServerEvent
{
    public class OperationCrossfireStopCommand : ICommand
    {
        public string Command { get; } = "OperationCrossfireStop";
        public string[] Aliases { get; } = { "OfcStop" };
        public string Description { get; } = "Stops the Operation Crossfire event if it is running.";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
        {
            if (!OperationCrossfireEventHandlers.OcfStarted)
            {
                response = "Operation Crossfire event is not currently running.";
                return false;
            }
            OperationCrossfireEventHandlers.OcfStarted = false;
            OperationCrossfireEventHandlers.EndEvent();
            response = "Operation Crossfire event has been stopped.";
            return true;
        }
    }
}