using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using VVUP.Base;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class DoorPicking : ActiveAbility
    {
        public override string Name { get; set; } = "Door Picking Ability";
        public override string Description { get; set; } = "Allows you to open any door for a short period of time, but limited by some external factors";
        public override float Duration { get; set; } = 15f;
        public override float Cooldown { get; set; } = 180f;
        public float TimeToDoorPickMin { get; set; } = 3;
        public float TimeToDoorPickMax { get; set; } = 6;
        public float TimeForDoorToBeOpen { get; set; } = 5f;
        public string BeforePickingDoorText { get; set; } = "Interact with a door to start to pick it";
        public string PickingDoorText { get; set; } = "Picking door...";
        [Description("Duration is unused here, it's based on the time it takes to pick the door")]
        public List<ApplyEffects> EffectsToApply { get; set; } = new List<ApplyEffects>
        {
            new()
            {
                EffectType = EffectType.Ensnared,
                Intensity = 1,
                Duration = 0,
            },
            new()
            {
                EffectType = EffectType.Slowness,
                Intensity = 255,
                Duration = 0,
            },
        };
        private bool pickingDoor = false;
        private List<Player> playerWithDoorPicking = new List<Player>();
        protected override void AbilityUsed(Player player)
        {
            player.ShowHint(BeforePickingDoorText, 5f);
            pickingDoor = true;
        }

        protected override void AbilityAdded(Player player)
        {
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            playerWithDoorPicking.Add(player);
            
        }

        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            playerWithDoorPicking.Remove(player);
            pickingDoor = false;
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!playerWithDoorPicking.Contains(ev.Player))
                return;
            
            if (ev.Door.IsOpen)
                return;

            if (ev.Player.CurrentItem != null)
                return;
            
            if (!pickingDoor)
                return;
            
            Log.Debug("VVUP Custom Abilities: Door Picking Ability, processing methods");
            ev.IsAllowed = false;
            float randomTime = GetRandomNumber.GetRandomFloat(TimeToDoorPickMin, TimeToDoorPickMax);
            ev.Player.ShowHint(PickingDoorText, randomTime);
            foreach (var effect in EffectsToApply)
            {
                ev.Player.EnableEffect(effect.EffectType, effect.Intensity, randomTime);
            }

            Timing.CallDelayed(randomTime, () =>
            {
                Log.Debug($"VVUP Custom Abilities: Opening {ev.Door.Name}");
                ev.Door.IsOpen = true;
                pickingDoor = false;
                Timing.CallDelayed(TimeForDoorToBeOpen, () =>
                {
                    ev.Door.IsOpen = false;
                });
            });
        }
    }
}