using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class ScaleAbility : PassiveAbility
    {
        public override string Name { get; set; } = "Scale Ability";

        public override string Description { get; set; } =
            "Handles everything in regards to custom roles with scaling";
            
        public List<Player> PlayersWithScaleAbility = new List<Player>();
        public Vector3 ScaleForPlayers { get; set; } = new Vector3(1f, 1f, 1f);
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Scale Ability, Adding Scale Ability to {player.Nickname}, Scale: {ScaleForPlayers}");
            Timing.CallDelayed(2.5f, () =>
            {
                player.Scale = ScaleForPlayers;
                player.SetScale(ScaleForPlayers);
            });
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Scale Ability, Removing Scale Ability from {player.Nickname}");
            player.Scale = Vector3.one;
            player.SetScale(Vector3.one);
        }
    }
}