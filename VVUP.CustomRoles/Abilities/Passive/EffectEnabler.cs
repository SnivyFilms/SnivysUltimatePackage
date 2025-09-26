using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;
using VVUP.Base;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class EffectEnabler : PassiveAbility
    {
        public override string Name { get; set; } = "Effect Enabler";
        public override string Description { get; set; } = "Enables Effects to the player";
        
        public List<ApplyEffects> EffectsToApply = new List<ApplyEffects>
        {
            new()
            {
                EffectType = EffectType.Invigorated
            }
        };
        
        protected override void AbilityAdded(Player player)
        {
            Timing.CallDelayed(5f, () =>
            {
                foreach (var effect in EffectsToApply)
                {
                    Log.Debug($"VVUP Custom Abilities: Activating {effect.EffectType} to {player.Nickname}");
                    player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
                }
            });
        }

        protected override void AbilityRemoved(Player player)
        {
            foreach (var effect in EffectsToApply)
            {
                Log.Debug($"VVUP Custom Abilities: Removing {effect.EffectType} from {player.Nickname}");
                player.DisableEffect(effect.EffectType);
            }
        }
    }
}