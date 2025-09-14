using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using VVUP.CustomItems.Items.Firearms;

namespace VVUP.CustomItems.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class GrenadeLauncherCollider : ICommand
    {
        public string Command { get; } = "GrenadeLauncherLaunchMode";
        public string[] Aliases { get; } = { "gllm", "launchmode" };
        public string Description { get; } = "Toggles if the grenade launcher uses impact or rollers";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not Player player)
            {
                response = "This command can only be executed by a player.";
                return false;
            }
            if (CustomItem.TryGet(player.CurrentItem, out var customItem) && customItem is GrenadeLauncher grenadeLauncher)
            {
                grenadeLauncher.ToggleLaunchTypeMode(player);
                response = "Launch mode toggled.";
                return true;
            }
            response = "You need to be holding a Grenade Launcher to use this command.";
            return false;
        }
    }
}