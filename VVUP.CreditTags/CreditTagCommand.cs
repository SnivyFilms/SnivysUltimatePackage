using CommandSystem;
using Exiled.API.Features;

namespace VVUP.CreditTags
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class CreditTagCommand : ICommand
    {
        public string Command { get; } = "vvupcredittag";
        public string[] Aliases { get; } = { "vvupcr", "vvupct" };

        public string Description { get; } =
            "Checks if you have a credit tag, and if so, applies it to the server because you have +1 bonus ducks";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandSender commandSender = (CommandSender)sender;
            if (Player.Get(commandSender.SenderId) is not Player player)
            {
                response = "You're still authenticating, try again once you authenticate to the server";
                return false;
            }

            bool found = Plugin.Instance.EventHandlers.SetRank(player, true);
            response = found ? "Your tag has been applied" : "You do not have a tag";
            return true;
        }
    }
}