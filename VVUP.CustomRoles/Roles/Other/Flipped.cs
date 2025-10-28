using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Other
{
    public class Flipped : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 0;

        public StartTeam StartTeam { get; set; } = StartTeam.Other;

        public override uint Id { get; set; } = 37;

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "Flipped";

        public override string Description { get; set; } =
            "For the people who complains that being small is boring";

        public override string CustomInfo { get; set; } = "Flipped";

        public override bool KeepInventoryOnSpawn { get; set; } = true;

        public override bool KeepRoleOnDeath { get; set; } = true;

        public override bool RemovalKillsPlayer { get; set; } = false;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new ScaleAbility()
            {
                Name = "Flipped Ability [Passive]",
                Description = "Handles being upside down",
                ScaleForPlayers = new Vector3(1, -1, 1),
            },
        };
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}