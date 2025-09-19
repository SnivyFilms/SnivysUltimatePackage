using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class Unspectatable : PassiveAbility
    {
        public override string Name { get; set; } = "Unspectatable";

        public override string Description { get; set; } =
            "Makes it so that you're unable to be spectated by spectators";
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Unspectatable, Adding Unspectatable Ability to {player.Nickname}");
            player.IsSpectatable = false;
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Unspectatable, Removing Unspectatable Ability from {player.Nickname}");
            player.IsSpectatable = true;
        }
    }
}