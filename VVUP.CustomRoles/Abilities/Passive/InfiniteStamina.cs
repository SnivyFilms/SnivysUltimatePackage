using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class InfiniteStamina : PassiveAbility
    {
        public override string Name { get; set; } = "Infinite Stamina";
        public override string Description { get; set; } = "Disables stamina";
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Infinite Stamina, Adding Infinite Stamina to {player.Nickname}");
            player.IsUsingStamina = false;
            base.AbilityAdded(player);
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Infinite Stamina, Removing Infinite Stamina from {player.Nickname}");
            player.IsUsingStamina = true;
            base.AbilityRemoved(player);
        }
    }
}