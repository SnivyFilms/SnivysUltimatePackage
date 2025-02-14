using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;
using UnityEngine;

namespace SnivysUltimatePackageOneConfig.Custom.Abilities.Passive
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
            Log.Debug($"VVUP Custom Abilities: Dwarf, Adding Dwarf Ability to {player.Nickname}");
            PlayersWithScaleAbility.Add(player);
            Timing.CallDelayed(2.5f, () => player.Scale = ScaleForPlayers);
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Dwarf, Removing Dwarf Ability from {player.Nickname}");
            PlayersWithScaleAbility.Remove(player);
            player.Scale = Vector3.one;
        }
    }
}