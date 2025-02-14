﻿using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using SnivysUltimatePackageOneConfig.API;
using SnivysUltimatePackageOneConfig.Custom.Abilities.Passive;

namespace SnivysUltimatePackageOneConfig.Custom.Roles.Scientist
{
    [CustomRole(RoleTypeId.Scientist)]
    public class ContainmentScientist : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 100;

        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;

        public override uint Id { get; set; } = 30;

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#FFFF7C>Containment Engineer Scientist</color>";

        public override string Description { get; set; } =
            @"\n\n\n\nA scientist that starts in Entrance Zone, with a different loadout\nYou cannot escape and become a MTF member.\nYou can be detained and escape to become a CI member.";

        public override string CustomInfo { get; set; } = "Containment Engineer Scientist";

        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>()
            {
                new()
                {
                    Role = RoleTypeId.FacilityGuard,
                    Chance = 100,
                },
            },
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Medkit.ToString(),
            ItemType.KeycardContainmentEngineer.ToString(),
            ItemType.Adrenaline.ToString(),
            ItemType.Radio.ToString(),
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new RestrictedEscape
            {
                Name = "Restricted Escape [Passive]",
                Description = "Restricts Escaping as Containment Engineer Scientist",
                EscapeText = "As a Containment Engineer Scientist, you''re unable to escape, unless you''re detained.",
                EscapeTextTime = 5,
                UseHints = true,
            },
        };
    }
}