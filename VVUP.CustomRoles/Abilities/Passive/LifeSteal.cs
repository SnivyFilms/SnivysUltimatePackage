using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class LifeSteal : PassiveAbility
    {
        public override string Name { get; set; } = "Life Steal";

        public override string Description { get; set; } =
            "When dealing damage to a player, you are able to heal a set percentage of the damage dealt.";
        
        public float LifeStealPercentage { get; set; } = 0.1f;
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: LifeSteal, Adding LifeSteal Ability to {player.Nickname}");
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.AbilityAdded(player);
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: LifeSteal, Removing LifeSteal Ability from {player.Nickname}");
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.AbilityRemoved(player);
        }
        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null)
                return;
            
            if (ev.Attacker.IsAlive && ev.Player.IsAlive && ev.Attacker != ev.Player && Check(ev.Attacker))
            {
                ev.Attacker.Heal(ev.Amount * LifeStealPercentage);
                Log.Debug($"VVUP Custom Abilities: LifeSteal, {ev.Attacker.Nickname} healed for {ev.Amount * LifeStealPercentage} health from {ev.Player.Nickname}");
            }
        }
    }
}