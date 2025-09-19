using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Scientist
{
    [CustomRole(RoleTypeId.Scientist)]
    public class QuantumGravityResearcher : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;
        public override uint Id { get; set; } = 62;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#FFFF7C>Quantum Gravity Researcher</color>";
        public override string Description { get; set; } = "A Scientist who researches quantum gravity.";
        public override string CustomInfo { get; set; } = "Quantum Gravity Researcher";

        public override List<string> Inventory { get; set; } = new()
        {
            "<color=#6600CC>Lunar Lob</color>",
            "<color=#6600CC>Nebula Carapace</color>",
            ItemType.Painkillers.ToString(),
            ItemType.KeycardJanitor.ToString()
        };

        public override string AbilityUsage { get; set; } = "You have no custom abilities";
    }
}