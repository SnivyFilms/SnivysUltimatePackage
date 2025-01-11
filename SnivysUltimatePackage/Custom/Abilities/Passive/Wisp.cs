﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using MEC;

namespace SnivysUltimatePackage.Custom.Abilities.Passive
{
    [CustomAbility]
    public class Wisp : PassiveAbility
    {
        public override string Name { get; set; } = "Wisp";

        public override string Description { get; set; } = "Enables walking through doors, Fog Control, Reduced Sprint";

        public Dictionary<EffectType, byte> EffectsToApply { get; set; } = new Dictionary<EffectType, byte>()
        {
            {EffectType.Exhausted, 1},
            {EffectType.Ghostly, 1},
            {EffectType.FogControl, 5},
        };
        
        protected override void AbilityAdded(Player player)
        {
            Timing.CallDelayed(10f, () =>
            {
                foreach (var effect in EffectsToApply)
                {
                    Log.Debug("VVUP Custom Abilities: Activating Wisp Effects");
                    player.EnableEffect(effect.Key, effect.Value, 0);
                }
            });
        }

        protected override void AbilityRemoved(Player player)
        {
            foreach (var effect in EffectsToApply)
            {
                Log.Debug("VVUP Custom Abilities: Removing Wisp Effects");
                player.DisableEffect(effect.Key);
            }
        }
    }
}