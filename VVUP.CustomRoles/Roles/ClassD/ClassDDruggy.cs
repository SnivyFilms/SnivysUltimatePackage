using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.ClassD
{
    public class ClassDDruggy : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override uint Id { get; set; } = 63;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#FF8E00>Class-D Druggy</color>";

        public override string Description { get; set; } =
            "A Class-D who is critically addicted to drugs.";

        public override string CustomInfo { get; set; } = "Class-D Druggy";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Painkillers.ToString(),
            ItemType.Painkillers.ToString(),
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new Addicted
            {
                Name = "Addicted [Passive]",
            },
        };
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
        
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}