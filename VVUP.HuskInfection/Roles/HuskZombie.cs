using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;
using VVUP.HuskInfection.Abilities;
using YamlDotNet.Serialization;

namespace VVUP.HuskInfection.Roles
{
    public class HuskZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 0;
        public override uint Id { get; set; } = 56;
        public override int MaxHealth { get; set; } = 350;
        public override string Name { get; set; } = "<color=#FF0000>Husk</color>";
        public override string Description { get; set; } = "Unlike other zombies, this is a Husk, came from someone who was infected something giving a living player a growing husk inside of them, which then took over.";
        public override string CustomInfo { get; set; } = "Husk";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Other;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
           new ApplyHuskInfection()
           {
               Name = "Apply Husk Infection On Hit [Passive]",
           },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 0,
        };
        
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}