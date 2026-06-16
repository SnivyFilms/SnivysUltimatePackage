using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using UnityEngine;
using PlayerLab = LabApi.Features.Wrappers.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class GravityAdjuster : PassiveAbility
    {
        public override string Name { get; set; } = "Gravity Adjuster";
        public override string Description { get; set; } = "Adjusts the player's gravity";
        public Vector3 GravityAdjustments { get; set; } = new Vector3(0, -12.3f, 0);
        public Dictionary<Player, Vector3> OriginalGravity = new Dictionary<Player, Vector3>();

        protected override void AbilityAdded(Player player)
        {
            if (player.ReferenceHub == null)
                return;
            Log.Debug($"VVUP Custom Abilities, GravityAdjuster: Setting {player.Nickname} gravity to {GravityAdjustments}");
            OriginalGravity.Add(player, PlayerLab.Get(player.NetworkIdentity)!.Gravity);
            PlayerLab.Get(player.NetworkIdentity)!.Gravity = GravityAdjustments;
            base.AbilityAdded(player);
        }
        protected override void AbilityRemoved(Player player)
        {
            if (player.ReferenceHub == null)
                return;
            Log.Debug($"VVUP Custom Abilities, GravityAdjuster: Resetting {player.Nickname} gravity to what they had previously");
            PlayerLab.Get(player.NetworkIdentity)!.Gravity = OriginalGravity.TryGetValue(player, out Vector3 originalGravity) ? originalGravity : new Vector3(0, -19.6f, 0);
            OriginalGravity.Remove(player);
            base.AbilityRemoved(player);
        }
    }
}