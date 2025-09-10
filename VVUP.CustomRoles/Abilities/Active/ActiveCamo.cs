using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class ActiveCamo : ActiveAbility
    {
        public override string Name { get; set; } = "Active Camo";

        public override string Description { get; set; } =
        "Activates a camouflage effect that acts like SCP-268 but doesn't break on interacting with objects, only when shooting.";

        public override float Duration { get; set; } = 30f;

        public override float Cooldown { get; set; } = 120f;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Interacted += OnInteracted;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingElevator += OnInteractingElevator;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInteractingLocker;
            base.SubscribeEvents();
        }

        protected override void AbilityUsed(Player player)
        {
            try
            {
                Log.Debug($"{Name} enabled for {Duration}");
                player.EnableEffect(EffectType.Invisible, Duration);
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.Interacted -= OnInteracted;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingElevator -= OnInteractingElevator;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractingLocker;
            foreach (Player player in ActivePlayers.ToList())
                EndAbility(player);
            base.UnsubscribeEvents();
        }

        protected override void AbilityEnded(Player player)
        {
            Log.Debug($"{Name} ended.");
            player.DisableEffect(EffectType.Invisible);
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (Check(ev.Player))
                EndAbility(ev.Player);
        }

        private void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (Check(ev.Player))
                Timing.CallDelayed(0.25f, () => AbilityUsed(ev.Player));
        }

        private void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (Check(ev.Player))
                Timing.CallDelayed(0.25f, () => AbilityUsed(ev.Player));
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            Log.Debug(Check(ev.Player));
            if (Check(ev.Player))
                Timing.CallDelayed(0.25f, () => AbilityUsed(ev.Player));
        }

        private void OnInteracted(InteractedEventArgs ev)
        {
            Log.Debug(Check(ev.Player));
            if (Check(ev.Player))
                Timing.CallDelayed(0.25f, () => AbilityUsed(ev.Player));
        }
    }
}