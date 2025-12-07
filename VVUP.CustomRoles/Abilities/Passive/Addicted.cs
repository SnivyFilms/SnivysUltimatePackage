using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using MEC;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class Addicted : PassiveAbility
    {
        public override string Name { get; set; } = "Addicted";
        public override string Description { get; set; } = "Makes the player addicted to pills and adrenaline. Adverse effects occur when waiting too long between uses.";
        [Description("Message shown to the player when they are experiencing withdrawal symptoms.")]
        public string MessageOnWithdrawal { get; set; } = "You feel terrible withdrawal symptoms coursing through your body!";

        [Description("Message shown to the player when they take an addicted item.")]
        public string MessageOnUse { get; set; } = "You feel better for now.";

        public float MessageDuration { get; set; } = 5f;
        public bool UseHints { get; set; } = true;
        
        [Description("The time in seconds before withdrawal symptoms start.")]
        public float TimeBeforeWithdrawals { get; set; } = 90f;
        [Description("The interval in seconds at which withdrawal symptoms will be applied.")]
        public float WithdrawalInterval { get; set; } = 15f;
        [Description("The amount of damage dealt to the player each withdrawal interval.")]
        public float WithdrawalAmount { get; set; } = 5f;

        [Description(
            "Should sedating withdrawls for a valid item take place on Using Item (when a player left clicks) or on Using Item Complete (after the animation is played)?")]
        public bool SedateOnUsingItemComplete { get; set; } = true;
        
        [Description("Regular items that will tame the effects of the addiction.")]
        [CanBeNull]
        public List<ItemType> AddictedItems { get; set; } = new List<ItemType>
        {
            ItemType.Adrenaline,
            ItemType.Painkillers,
        };

        [Description("Custom items that will tame the effects of the addiction. Custom Items by ID")]
        [CanBeNull]
        public List<uint> CustomAddictedItems { get; set; } = new List<uint>
        {
            23,
            34,
        };
        
        private Dictionary<Player, CoroutineHandle> addictedPlayers = new Dictionary<Player, CoroutineHandle>();
        protected override void AbilityAdded(Player player)
        {
            if (SedateOnUsingItemComplete)
                Exiled.Events.Handlers.Player.UsingItemCompleted += OnUsingItemCompleted;
            else
                Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            
            CoroutineHandle handle = Timing.RunCoroutine(WithdrawalCoroutine(player));
            addictedPlayers.Add(player, handle);
            base.AbilityAdded(player);
        }
        
        protected override void AbilityRemoved(Player player)
        {
            if (SedateOnUsingItemComplete)
                Exiled.Events.Handlers.Player.UsingItemCompleted -= OnUsingItemCompleted;
            else
                Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            if (addictedPlayers.ContainsKey(player))
            {
                Timing.KillCoroutines(addictedPlayers[player]);
                addictedPlayers.Remove(player);
            }
            base.AbilityRemoved(player);
        }

        private void OnUsingItemCompleted(UsingItemCompletedEventArgs ev)
        {
            if (Check(ev.Player) && ShouldSedateWithdrawal(ev.Item))
            {
                ResetWithdrawal(ev.Player);
            }
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (Check(ev.Player) && ShouldSedateWithdrawal(ev.Item))
            {
                ResetWithdrawal(ev.Player);
            }
        }
        
        private IEnumerator<float> WithdrawalCoroutine(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Addicted, Starting withdrawal coroutine for {player.Nickname}, waiting {TimeBeforeWithdrawals} seconds");
            yield return Timing.WaitForSeconds(TimeBeforeWithdrawals);
            
            while (player.IsAlive)
            {
                player.Hurt(WithdrawalAmount);
                if (!string.IsNullOrWhiteSpace(MessageOnWithdrawal))
                    if (UseHints)
                        player.ShowHint(MessageOnWithdrawal, MessageDuration);
                    else
                        player.Broadcast((ushort)MessageDuration, MessageOnWithdrawal);
                
                yield return Timing.WaitForSeconds(WithdrawalInterval);
            }
        }
        
        private bool ShouldSedateWithdrawal(Item item)
        {
            return (AddictedItems != null && AddictedItems.Contains(item.Type)) || 
                   (CustomAddictedItems != null && CustomItem.TryGet(item, out CustomItem customItem) && 
                    CustomAddictedItems.Contains(customItem.Id));
        }

        private void ResetWithdrawal(Player player)
        {
            if (addictedPlayers.ContainsKey(player))
            {
                Log.Debug($"VVUP Custom Abilities: Addicted, {player.Nickname} used an addicted item, resetting withdrawal timer.");
                Timing.KillCoroutines(addictedPlayers[player]);
                CoroutineHandle newHandle = Timing.RunCoroutine(WithdrawalCoroutine(player));
                addictedPlayers[player] = newHandle;
        
                if (!string.IsNullOrWhiteSpace(MessageOnUse))
                    if (UseHints)
                        player.ShowHint(MessageOnUse, MessageDuration);
                    else
                        player.Broadcast((ushort)MessageDuration, MessageOnUse);
            }
        }
    }
}