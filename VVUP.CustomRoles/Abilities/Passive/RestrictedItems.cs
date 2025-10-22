using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class RestrictedItems : PassiveAbility
    {
        public override string Name { get; set; } = "Restricted Items";
        public override string Description { get; set; } = "Handles restricted items";

        public List<ItemType> RestrictedItemList { get; set; } = new List<ItemType>();
        public bool RestrictUsingItems { get; set; } = true;
        public bool RestrictPickingUpItems { get; set; } = true;
        public bool RestrictDroppingItems { get; set; } = true;

        protected override void AbilityAdded(Player player)
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
        }

        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
        }
        
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!RestrictUsingItems)
                return;
            if (Check(ev.Player) && RestrictedItemList != null &&
                RestrictedItemList.Contains(ev.Item.Type))
            {
                Log.Debug($"VVUP Custom Abilities: Restricting {ev.Player.Nickname} from using up {ev.Item}");
                ev.IsAllowed = false;
            }
        }

        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!RestrictPickingUpItems)
                return;
            if (Check(ev.Player) && RestrictedItemList != null &&
                RestrictedItemList.Contains(ev.Pickup.Type))
            {
                Log.Debug($"VVUP Custom Abilities: Restricting {ev.Player.Nickname} from picking up {ev.Pickup}");
                ev.IsAllowed = false;
            }
        }
        private void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!RestrictDroppingItems)
                return;
            if (Check(ev.Player) && RestrictedItemList != null &&
                RestrictedItemList.Contains(ev.Item.Type))
            {
                Log.Debug($"VVUP Custom Abilities: Restricting {ev.Player.Nickname} from dropping {ev.Item}");
                ev.IsAllowed = false;
            }
        }
    }
}