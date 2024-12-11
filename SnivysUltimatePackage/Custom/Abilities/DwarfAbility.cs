﻿using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using MEC;
using UnityEngine;

namespace SnivysUltimatePackage.Custom.Abilities
{
    [CustomAbility]
    public class DwarfAbility : PassiveAbility
    {
        public override string Name { get; set; } = "DwarfAbility";

        public override string Description { get; set; } =
            "Handles everything in regards to being a dwarf";
            
        public List<Player> PlayersWithDwarfEffect = new List<Player>();
        
        protected override void AbilityAdded(Player player)
        {
            PlayersWithDwarfEffect.Add(player);
            Timing.CallDelayed(2.5f, () => player.Scale = new Vector3(0.75f, 0.75f, 0.75f));
            player.IsUsingStamina = false;
        }
        protected override void AbilityRemoved(Player player)
        {
            PlayersWithDwarfEffect.Remove(player);
            player.Scale = Vector3.one;
            player.IsUsingStamina = true;
        }
    }
}