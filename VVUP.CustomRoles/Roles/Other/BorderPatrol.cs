using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Other
{
    public class BorderPatrol : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 0;

        public StartTeam StartTeam { get; set; } = StartTeam.Other;

        public override uint Id { get; set; } = 34;

        public override RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "Border Patrol";

        public override string Description { get; set; } =
            "You have been religated to ensuring safe passage from Heavy and Enterance Zone";

        public override string CustomInfo { get; set; } = "Border Patrol";

        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.GunE11SR.ToString(),
        };
    }
}