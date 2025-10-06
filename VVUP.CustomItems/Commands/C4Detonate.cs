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
        public string Description { get; } = "Detonate command for detonating C4 & F4 charges";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(sender);
            bool hasC4Charges = Items.Grenades.C4.PlacedCharges.ContainsValue(ply);
            bool hasF4Charges = Items.Grenades.F4.PlacedCharges.ContainsValue(ply);
            
            if (!hasC4Charges && !hasF4Charges)
            {
                response = "\n<color=red>You haven't placed any charges!</color>";
                return false;
            }

            if (Items.Grenades.C4.Instance.RequireDetonator && (ply.CurrentItem is null || ply.CurrentItem.Type != Items.Grenades.C4.Instance.DetonatorItem))
            {
                response = $"\n<color=red>You need to have a Remote Detonator ({Items.Grenades.C4.Instance.DetonatorItem}) in your hand to detonate C4!</color>";
                return false;
            }

            bool needsC4Detonator = Items.Grenades.C4.Instance.RequireDetonator && hasC4Charges;
            bool needsF4Detonator = Items.Grenades.F4.Instance.RequireDetonator && hasF4Charges;
            
            if ((needsC4Detonator || needsF4Detonator) && 
                (ply.CurrentItem is null || 
                (needsC4Detonator && ply.CurrentItem.Type != Items.Grenades.C4.Instance.DetonatorItem) ||
                (needsF4Detonator && ply.CurrentItem.Type != Items.Grenades.F4.Instance.DetonatorItem)))
            {
                response = "\n<color=red>You need to have a Remote Detonator in your hand to detonate charges!</color>";
                return false;
            }

            int c4Count = 0;
            int f4Count = 0;
            //C4
            foreach (var charge in Items.Grenades.C4.PlacedCharges.ToList())
            {
                if (charge.Value != ply)
                    continue;

                float distance = Vector3.Distance(charge.Key.Position, ply.Position);

                if (distance < Items.Grenades.C4.Instance.MaxDistance)
                {
                    Items.Grenades.C4.Instance.C4Handler(charge.Key);
                    c4Count++;
                }
                else
                {
                    ply.SendConsoleMessage($"One of your C4 charges is out of range. You need to get closer by {Mathf.Round(distance - Items.Grenades.C4.Instance.MaxDistance)} meters.", "yellow");
                }
            }
            //F4
            foreach (var charge in Items.Grenades.F4.PlacedCharges.ToList())
            {
                if (charge.Value != ply)
                    continue;

                float distance = Vector3.Distance(charge.Key.Position, ply.Position);

                if (distance < Items.Grenades.F4.Instance.MaxDistance)
                {
                    Items.Grenades.F4.Instance.F4Handler(charge.Key);
                    f4Count++;
                }
                else
                {
                    ply.SendConsoleMessage($"One of your F4 charges is out of range. You need to get closer by {Mathf.Round(distance - Items.Grenades.F4.Instance.MaxDistance)} meters.", "yellow");
                }
            }

            int totalCharges = c4Count + f4Count;
            response = totalCharges == 1 
                ? $"\n<color=green>{totalCharges} charge has been detonated!</color>" 
                : $"\n<color=green>{totalCharges} charges have been detonated!</color>";

            return true;
        }
    }
}