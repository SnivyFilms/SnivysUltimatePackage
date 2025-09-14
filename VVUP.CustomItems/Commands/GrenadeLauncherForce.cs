using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using VVUP.CustomItems.Items.Firearms;

namespace VVUP.CustomItems.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class GrenadeLauncherForce : ICommand
    {
        public string Command { get; } = "GrenadeLauncherForceMode";
        public string[] Aliases { get; } = { "glfm", "forcemode" };
        public string Description { get; } = "Toggles if the grenade launcher force";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not Player player)
            {
                response = "This command can only be executed by a player.";
                return false;
            }
            if (CustomItem.TryGet(player.CurrentItem, out var customItem) && customItem is GrenadeLauncher grenadeLauncher)
            {
                grenadeLauncher.ToggleForceMode(player);
                response = "Force mode toggled.";
                return true;
            }
            response = "You need to be holding a Grenade Launcher to use this command.";
            return false;
        }
    }
}