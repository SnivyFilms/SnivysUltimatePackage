using System;
using CommandSystem;

namespace VVUP.Base
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ActiveModulesCommand : ICommand
    {
        public string Command { get; } = "VVUPActiveModules";
        public string[] Aliases { get; } = new[] { "ActiveModules", "VVUPModules" };
        public string Description { get; } = "Prints out modules are active";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string activeModules = string.Empty;
            if (Plugin.Instance.VvupCi)
                activeModules += "Custom Items\n";
            if (Plugin.Instance.VvupCr)
                activeModules += "Custom Roles\n";
            if (Plugin.Instance.VvupFcr)
                activeModules += "Free Custom Roles\n";
            if (Plugin.Instance.VvupSe)
                activeModules += "Server Events\n";
            if (Plugin.Instance.VvupMdr)
                activeModules += "Micro Damage Reduction\n";
            if (Plugin.Instance.VvupWe)
                activeModules += "Weapon Evaporate\n";
            if (Plugin.Instance.VvupRs)
                activeModules += "Round Start\n";
            if (Plugin.Instance.VvupSc)
                activeModules += "SCP Changes\n";
            if (Plugin.Instance.VvupFa)
                activeModules += "Flamingo Adjustments\n";
            if (Plugin.Instance.VvupHk)
                activeModules += "Husk Infection\n";
            if (Plugin.Instance.VvupVo)
                activeModules += "Votes\n";
            if (Plugin.Instance.VvupCt)
                activeModules += "Credit Tags\n";
            if (activeModules == string.Empty)
                activeModules = "No modules has been loaded";
            response = $"VVUP Active Modules: {activeModules}";
            return true;
        }
    }
}