using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using UnityEngine;

namespace VVUP.CustomItems.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class C4Detonate : ICommand
    {
        public string Command { get; } = "detonate";
        public string[] Aliases { get; } = new string[] { "det" };
        public string Description { get; } = "Detonate command for detonating C4 charges";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(sender);

            if (!Items.Grenades.C4.PlacedCharges.ContainsValue(ply))
            {
                response = "\n<color=red>You've haven't placed any C4 charges!</color>";
                return false;
            }

            if (Items.Grenades.C4.Instance.RequireDetonator && (ply.CurrentItem is null || ply.CurrentItem.Type != Items.Grenades.C4.Instance.DetonatorItem))
            {
                response = $"\n<color=red>You need to have a Remote Detonator ({Items.Grenades.C4.Instance.DetonatorItem}) in your hand to detonate C4!</color>";
                return false;
            }

            int i = 0;

            foreach (var charge in Items.Grenades.C4.PlacedCharges.ToList())
            {
                if (charge.Value != ply)
                    continue;

                float distance = Vector3.Distance(charge.Key.Position, ply.Position);

                if (distance < Items.Grenades.C4.Instance.MaxDistance)
                {
                    Items.Grenades.C4.Instance.C4Handler(charge.Key);

                    i++;
                }
                else
                {
                    ply.SendConsoleMessage($"One of your charges is out of range. You need to get closer by {Mathf.Round(distance - Items.Grenades.C4.Instance.MaxDistance)} meters.", "yellow");
                }
            }

            response = i == 1 ? $"\n<color=green>{i} C4 charge has been detonated!</color>" : $"\n<color=green>{i} C4 charges have been detonated!</color>";

            return true;
        }
    }
}