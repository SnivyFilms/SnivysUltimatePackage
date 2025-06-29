﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using SnivysUltimatePackageOneConfig.API;
using SnivysUltimatePackageOneConfig.Custom.Abilities.Passive;
using UnityEngine;

namespace SnivysUltimatePackageOneConfig.Custom.Roles.Scps
{
    public class PoisonousZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 20;
        public override uint Id { get; set; } = 54;
        public override int MaxHealth { get; set; } = 350;
        public override string Name { get; set; } = "<color=#FF0000>Poisonous Zombie</color>";
        public override string Description { get; set; } = "A zombie that when attacking a player, applies the poison effect to them.";
        public override string CustomInfo { get; set; } = "Poisonous Zombie";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ApplyEffectOnHit()
            {
                Name = "Poisonous Touch [Passive]",
                Description = "When you hit a player, they will be poisoned.",
                EffectsToApply = new Dictionary<EffectType, Dictionary<byte, float>>
                {
                    {EffectType.Poisoned, new Dictionary<byte, float> { { 1, 10f } } },
                },
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
        };
        
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
    }
}