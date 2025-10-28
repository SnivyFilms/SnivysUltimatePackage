using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Foundation
{
    public class Vanguard : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 10;
        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;
        public override uint Id { get; set; } = 48;
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfSpecialist;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#0096FF>MTF Vanguard</color>";
        public override string Description { get; set; } = "A MTF member with an alternate loadout. Being able to help mark priority targets.";
        public override string CustomInfo { get; set; } = "MTF Vanguard";
        
        public override List<string> Inventory { get; set; } = new()
        {
            "<color=#0096FF>Pathfinder</color>",
            "<color=#FF0000>Viper</color>",
            "<color=#6600CC>Pathfinder Grenade</color>",
            ItemType.KeycardMTFOperative.ToString(),
            ItemType.ArmorHeavy.ToString(),
            ItemType.Radio.ToString(),
            ItemType.Adrenaline.ToString(),
            ItemType.Painkillers.ToString(),
        };
        
        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato9, 120
            }
        };
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.NtfSpecialist,
                    Chance = 100,
                },
            },
        };
        
        public override string AbilityUsage { get; set; } = "You have no special abilities";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}