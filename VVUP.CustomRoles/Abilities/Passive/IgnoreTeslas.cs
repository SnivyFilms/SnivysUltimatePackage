using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class IgnoreTeslas : PassiveAbility
    {
        public override string Name { get; set; } = "Ignore Teslas";
        public override string Description { get; set; } = "Allows the player to ignore tesla gates";

        protected override void AbilityAdded(Player player)
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
            base.AbilityAdded(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
            base.AbilityRemoved(player);
        }
        
        private void OnHurting(HurtingEventArgs ev)
        {
            if (Check(ev.Player) && ev.DamageHandler.Type == DamageType.Tesla)
            {
                Log.Debug($"VVUP Custom Abilities, Ignore Teslas: Prevented Tesla damage to {ev.Player.Nickname}");
                ev.IsAllowed = false;
            }
        }
        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (Check(ev.Player))
            {
                Log.Debug($"VVUP Custom Abilities, Ignore Teslas: Prevented Tesla triggering for {ev.Player.Nickname}");
                ev.IsAllowed = false;
            }
        }
    }
}