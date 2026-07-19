using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Roles;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Scp173;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class SoundBreaker : ActiveAbility
    {
        public override string Name { get; set; } = "Sound Breaker";
        public override string Description { get; set; } = "Reset blink cooldown, reduce the next Blink interval and range";
        public override float Duration { get; set; } = 5f;
        public override float Cooldown { get; set; } = 150f;
        public float BlinkCooldown { get; set; } = 0.5f;
        public float MaxBlinkDistance { get; set; } = 4f;
        public int MinimumObservedPlayers { get; set; } = 1;

        public override bool CanUseAbility(Player player, out string response, bool selectedOnly = false)
        {
            if (player.Role is Scp173Role scp173Role)
            {
                int observers = scp173Role.ObservingPlayers.Count();

                if (observers >= MinimumObservedPlayers)
                    return base.CanUseAbility(player, out response, selectedOnly);
            }

            response = "No one is watching you.";
            return false;
        }

        protected override void AbilityUsed(Player player)
        {
            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

            if (player.Role is Scp173Role role)
                role.BlinkCooldown = 0f;
        }

        protected override void AbilityEnded(Player player)
        {
            Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;

            player.ShowHint("Sound Breaker ended.", 3f);
        }

        private void OnBlinking(BlinkingEventArgs ev)
        {
            if (ActivePlayers.Contains(ev.Player))
            {
                ev.BlinkCooldown = BlinkCooldown;

                Vector3 current = ev.Player.Position;
                Vector3 target = ev.BlinkPosition;
                Vector3 direction = (target - current);

                if (direction.magnitude > MaxBlinkDistance)
                {
                    direction = direction.normalized * MaxBlinkDistance;
                    ev.BlinkPosition = current + direction;
                }
            }
        }
    }
}