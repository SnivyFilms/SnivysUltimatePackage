using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
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
        public override string Name { get; set; } = "<color=#FFFF7C>Tesla Technician</color>";
        public override string Description { get; set; } = "A Scientist who routinely works on Tesla Gates.";
        public override string CustomInfo { get; set; } = "Tesla Technician";

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardScientist.ToString(),
            ItemType.Medkit.ToString(),
        };
        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new IgnoreTeslas()
            {
                Name = "Ignore Teslas [Passive]",
            },
        };
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
    }
}