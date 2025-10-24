using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Scientist
{
    public class TeslaTechnician : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;
        public override uint Id { get; set; } = 64;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#FFFF7C>Quantum Gravity Researcher</color>";
        public override string Description { get; set; } = "A Scientist who researches quantum gravity.";
        public override string CustomInfo { get; set; } = "Quantum Gravity Researcher";

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardScientist.ToString(),
            ItemType.Medkit.ToString(),
        };

        public override string AbilityUsage { get; set; } = "You have no custom abilities";
    }
}