using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class RestrictedItems : PassiveAbility
    {
        public override string Name { get; set; } = "Restricted Items";
        public override string Description { get; set; } = "Handles restricted items";

        public List<uint> AllowedCustomItems { get; set; } = new List<uint>();
        public List<uint> RestrictedCustomItems { get; set; } = new List<uint>();
        public List<ItemType> RestrictedItemList { get; set; } = new List<ItemType>();
        public bool RestrictUsingItems { get; set; } = true;
        public bool RestrictPickingUpItems { get; set; } = true;
        public bool RestrictDroppingItems { get; set; } = true;

        protected override void AbilityAdded(Player player)
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            base.AbilityAdded(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            base.AbilityRemoved(player);
        }
        
        private bool IsItemRestricted(ItemType itemType, Item item)
        {
            if (CustomItem.TryGet(item, out CustomItem customItem))
            {
                if (AllowedCustomItems.Count > 0 && AllowedCustomItems.Contains(customItem.Id))
                    return false;
                
                if (RestrictedCustomItems.Contains(customItem.Id))
                    return true;
            }

            return RestrictedItemList != null && RestrictedItemList.Contains(itemType);
        }

        private bool IsPickupRestricted(ItemType itemType, Pickup pickup)
        {
            if (CustomItem.TryGet(pickup, out CustomItem customItem))
            {
                if (AllowedCustomItems.Count > 0 && AllowedCustomItems.Contains(customItem.Id))
                    return false;
                
                if (RestrictedCustomItems.Contains(customItem.Id))
                    return true;
            }

            return RestrictedItemList != null && RestrictedItemList.Contains(itemType);
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!RestrictUsingItems || !Check(ev.Player))
                return;

            if (IsItemRestricted(ev.Item.Type, ev.Item))
            {
                Log.Debug($"VVUP Custom Abilities: Restricting {ev.Player.Nickname} from using {ev.Item}");
                ev.IsAllowed = false;
            }
        }

        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!RestrictPickingUpItems || !Check(ev.Player))
                return;

            if (IsPickupRestricted(ev.Pickup.Type, ev.Pickup))
            {
                Log.Debug($"VVUP Custom Abilities: Restricting {ev.Player.Nickname} from picking up {ev.Pickup}");
                ev.IsAllowed = false;
            }
        }

        private void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!RestrictDroppingItems || !Check(ev.Player))
                return;

            if (IsItemRestricted(ev.Item.Type, ev.Item))
            {
                Log.Debug($"VVUP Custom Abilities: Restricting {ev.Player.Nickname} from dropping {ev.Item}");
                ev.IsAllowed = false;
            }
        }
    }
}