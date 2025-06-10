﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using SnivysUltimatePackage.API;
using SnivysUltimatePackage.Custom.Abilities.Passive;
using UnityEngine;

namespace SnivysUltimatePackage.Custom.Roles.ClassD
{
    public class ClassDTank : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 5;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override uint Id { get; set; } = 52;
        public override int MaxHealth { get; set; } = 300;
        public override string Name { get; set; } = "<color=#FF8E00>Class-D Tank</color>";
        public override string Description { get; set; } = "You're a Class-D with enhanced durability, able to withstand more damage than your peers.";
        public override string CustomInfo { get; set; } = "Class-D Tank";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new EffectEnabler
            {
                Name = "Move Speed Reduction [Passive]",
                Description = "Slows you down",
                EffectsToApply = new Dictionary<EffectType, byte>()
                {
                    {EffectType.Slowness, 30},
                },
            },
            new ScaleAbility
            {
                Name = "Scale Ability [Passive]",
                Description = "Increases your size",
                ScaleForPlayers = new Vector3(1.1f, 1.1f, 1.1f)
            },
            new CustomRoleEscape
            {
                Name = "Custom Role Escape [Passive]",
                Description = "If you escape as a Class-D Tank, this will make sure you get your inventory.",
                EscapeToRegularRole = true,
                RegularRole = RoleTypeId.ChaosConscript,
                AllowCuffedCustomRoleChange = false,
                AllowUncuffedCustomRoleChange = false,
                SaveInventory = true
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
    }
}