using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class PocketDimensionEscapeChance : PassiveAbility
    {
        public override string Name { get; set; } = "Pocket Dimension Escape Chance";
        public override string Description { get; set; } = "Changes the player escape change in the pocket dimension.";
        public Dictionary<Player, int> PlayersWithPocketDimensionEscapeChance = new Dictionary<Player, int>();
        public int EscapeChance { get; set; } = 50;
        public int AmountOfAllowedEscapes { get; set; } = 3;
        public string CustomDeathReason { get; set; } = "Your luck has ran out of escaping from the pocket dimension.";
        public bool RemoveTraumatizedOnEscape { get; set; } = true;

        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Pocket Dimension Escape Chance, Adding to {player.Nickname}");
            PlayersWithPocketDimensionEscapeChance.Add(player, 0);
            Exiled.Events.Handlers.Player.EscapingPocketDimension += OnEscapingPocketSuccess;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += OnEscapingPocketFailure;
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Pocket Dimension Escape Chance, Removing from {player.Nickname}");
            PlayersWithPocketDimensionEscapeChance.Remove(player);
            Exiled.Events.Handlers.Player.EscapingPocketDimension -= OnEscapingPocketSuccess;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= OnEscapingPocketFailure;
        }

        private void OnEscapingPocketSuccess(EscapingPocketDimensionEventArgs ev)
        {
            if (!PlayersWithPocketDimensionEscapeChance.ContainsKey(ev.Player))
                return;
            Log.Debug($"VVUP Custom Abilities: Pocket Dimension Escape Chance, Checking if {ev.Player.Nickname} hit the limit of allowed escapes. Limit = {AmountOfAllowedEscapes}, Current = {PlayersWithPocketDimensionEscapeChance[ev.Player]}");
            if (PlayersWithPocketDimensionEscapeChance.ContainsValue(AmountOfAllowedEscapes))
            {
                Log.Debug($"VVUP Custom Abilities: Pocket Dimension Escape Chance, {ev.Player.Nickname} hit the limit of custom escapes, killing them.");
                ev.Player.Kill(CustomDeathReason);
                ev.IsAllowed = false;
            }
            else if (EscapeChance >= Base.GetRandomNumber.GetRandomInt(101))
            {
                Log.Debug(
                    $"VVUP Custom Abilities: Pocket Dimension Escape Chance, {ev.Player.Nickname} has gotten an escape chance, teleporting them to 106's room");
                ev.IsAllowed = true;
                ev.TeleportPosition = Room.Get(RoomType.Hcz106).Position + Vector3.up;
                if (RemoveTraumatizedOnEscape)
                    Timing.CallDelayed(1f, () => ev.Player.DisableEffect(EffectType.Traumatized));
                PlayersWithPocketDimensionEscapeChance[ev.Player]++;
                Log.Debug($"VVUP Custom Abilities: Pocket Dimension Escape Chance, {ev.Player.Nickname} escaped from the Pocket Dimension (Success path).");
            }
        }
        private void OnEscapingPocketFailure(FailingEscapePocketDimensionEventArgs ev)
        {
            if (!PlayersWithPocketDimensionEscapeChance.ContainsKey(ev.Player))
                return;
            Log.Debug($"VVUP Custom Abilities: Pocket Dimension Escape Chance, Checking if {ev.Player.Nickname} hit the limit of allowed escapes. Limit = {AmountOfAllowedEscapes}, Current = {PlayersWithPocketDimensionEscapeChance[ev.Player]}");
            if (PlayersWithPocketDimensionEscapeChance.ContainsValue(AmountOfAllowedEscapes))
            {
                Log.Debug($"VVUP Custom Abilities: Pocket Dimension Escape Chance, {ev.Player.Nickname} hit the limit of custom escapes, killing them.");
                ev.Player.Kill(CustomDeathReason);
                ev.IsAllowed = true;
            }
            else if (EscapeChance >= Base.GetRandomNumber.GetRandomInt(101))
            {
                Log.Debug(
                    $"VVUP Custom Abilities: Pocket Dimension Escape Chance, {ev.Player.Nickname} has gotten an escape chance, teleporting them to 106's room");
                ev.IsAllowed = false;
                ev.Player.Position = Room.Get(RoomType.Hcz106).Position + Vector3.up;
                if (RemoveTraumatizedOnEscape)
                    Timing.CallDelayed(1f, () => ev.Player.DisableEffect(EffectType.Traumatized));
                PlayersWithPocketDimensionEscapeChance[ev.Player]++;
                Log.Debug($"VVUP Custom Abilities: Pocket Dimension Escape Chance, {ev.Player.Nickname} escaped from the Pocket Dimension (Failure path).");
            }
        }
    }
}