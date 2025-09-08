using System.Collections.Generic;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Other
{
    public class PaperMan : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 0;
        public StartTeam StartTeam { get; set; } = StartTeam.Other;
        public override uint Id { get; set; } = 61;
        public override int MaxHealth { get; set; } = 50;
        public override string Name { get; set; } = "Paper Man";
        public override string Description { get; set; } = "You're flat like paper";
        public override string CustomInfo { get; set; } = "Paper Man";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility
            {
                Name = "Scale [Passive]",
                Description = "Makes you flat like paper",
                ScaleForPlayers = new UnityEngine.Vector3(1, 1, 0.1f),
            },
        };
    }
}