using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using VVUP.Base;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class ApplyEffectOnHit : PassiveAbility
    {
        public override string Name { get; set; } = "Apply Effect On Hit";
        public override string Description { get; set; } = "Enables Effects to whoever you hit";
        public List<ApplyEffects> EffectsToApply { get; set; } = new List<ApplyEffects>
        {
            new()
            {
                EffectType = EffectType.Invigorated,
                Intensity = 1,
                Duration = 5,
            }
        };
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: ApplyEffectOnHit, Adding ApplyEffectOnHit Ability to {player.Nickname}");
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: ApplyEffectOnHit, Removing ApplyEffectOnHit Ability from {player.Nickname}");
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null)
                return;
            if (Check(ev.Attacker))
            {
                if (EffectsToApply.IsEmpty()) 
                    return;
                foreach (var effect in EffectsToApply)
                {
                    Log.Debug(
                        $"VVUP Custom Abilities: ApplyEffectOnHit, applying {effect.EffectType} with intensity {effect.Intensity} and duration {effect.Duration} to {ev.Player.Nickname} from {ev.Attacker.Nickname}");
                    ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration,
                        effect.AddDurationIfActive);
                }
            }
        }
    }
}