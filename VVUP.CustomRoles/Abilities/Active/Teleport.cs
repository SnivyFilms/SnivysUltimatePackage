using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Abilities.Active
{
    public class Teleport : ActiveAbility
    {
        public override string Name { get; set; } = "Teleport";

        public override string Description { get; set; } =
            "Sets a place for the player to teleport to, then next interaction will teleport them to that place.";

        public override float Duration { get; set; } = 0;
        [YamlIgnore] public override float Cooldown { get; set; }
        public float CooldownAfterDeployingTeleportSpot { get; set; } = 15f;
        public float CooldownAfterTeleporting { get; set; } = 120f;
        public string TeleportSpotSetMessage { get; set; } = "Teleport spot set! Use the ability again to teleport.";
        public string TeleportingMessage { get; set; } = "Teleporting to your teleport spot!";
        public bool AllowTeleportingAcrossZones { get; set; } = false;

        [Description("This will only show if AllowTeleportingAcrossZones is false.")]
        public string TeleportingAcrossZonesMessage { get; set; } = "You cannot teleport across zones!";

        public bool UseHints { get; set; } = false;
        public float MessageDuration { get; set; } = 5f;

        public bool TeleportSpotSet = false;

        public Dictionary<Player, Dictionary<Vector3, ZoneType>> TeleportSpot =
            new Dictionary<Player, Dictionary<Vector3, ZoneType>>();

        protected override void AbilityUsed(Player player)
        {
            if (TeleportSpotSet)
            {
                if (!AllowTeleportingAcrossZones && TeleportSpot.ContainsKey(player) &&
                    TeleportSpot[player].Values.FirstOrDefault() != player.Zone)
                {
                    Timing.CallDelayed(0.5f, () =>
                    {
                        if (!string.IsNullOrWhiteSpace(TeleportingAcrossZonesMessage))
                            if (UseHints)
                                player.ShowHint(TeleportingAcrossZonesMessage, MessageDuration);
                            else
                                player.Broadcast((ushort)MessageDuration, TeleportingAcrossZonesMessage,
                                    shouldClearPrevious: true);

                    });
                    Log.Debug(
                        $"VVUP Custom Abilities: Teleport, {player.Nickname} tried to teleport across zones, but it is not allowed.");
                    return;
                }

                player.Position = TeleportSpot[player].Keys.First();
                Log.Debug(
                    $"VVUP Custom Abilities: Teleport, {player.Nickname} teleported to their teleport spot at position {player.Position}, setting cooldown to {CooldownAfterTeleporting} seconds, showing message.");
                TeleportSpotSet = false;
                TeleportSpot.Remove(player);
                Cooldown = CooldownAfterTeleporting;
                Timing.CallDelayed(0.5f, () =>
                {
                    if (!string.IsNullOrWhiteSpace(TeleportingMessage))
                        if (UseHints)
                            player.ShowHint(TeleportingMessage, MessageDuration);
                        else
                            player.Broadcast((ushort)MessageDuration, TeleportingMessage, shouldClearPrevious: true);
                });
            }
            else
            {
                TeleportSpot.Add(player, new Dictionary<Vector3, ZoneType> { { player.Position, player.Zone } });
                Cooldown = CooldownAfterDeployingTeleportSpot;
                TeleportSpotSet = true;
                Timing.CallDelayed(0.5f, () =>
                {
                    if (!string.IsNullOrWhiteSpace(TeleportSpotSetMessage))
                        if (UseHints)
                            player.ShowHint(TeleportSpotSetMessage, MessageDuration);
                        else
                            player.Broadcast((ushort)MessageDuration, TeleportSpotSetMessage, shouldClearPrevious: true);
                });
                Log.Debug(
                    $"VVUP Custom Abilities: Teleport, Teleport spot set for {player.Nickname} at position {player.Position}, setting cooldown to {CooldownAfterDeployingTeleportSpot} seconds, showing message.");
            }
        }

        protected override void AbilityRemoved(Player player)
        {
            if (TeleportSpot.ContainsKey(player))
            {
                Log.Debug(
                    $"VVUP Custom Abilities: Teleport, Removing teleport spot for {player.Nickname} at position {TeleportSpot[player].Keys.First()}");
                TeleportSpot.Remove(player);
            }
        }
    }
}