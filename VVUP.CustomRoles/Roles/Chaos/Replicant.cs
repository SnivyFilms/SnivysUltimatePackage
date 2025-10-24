using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Chaos
{
    public class Replicant : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 5;
        public override uint Id { get; set; } = 59;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#008f1e>Chaos Replicant</color>";
        public override string Description { get; set; } = "A Chaos Insurgent that can create decoy and make recon";
        public override string CustomInfo { get; set; } = "Chaos Replicant";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosRifleman;

        public StartTeam StartTeam { get; set; } = StartTeam.Chaos;

        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.ArmorCombat}",
            $"{ItemType.Painkillers}",
            $"{ItemType.Medkit}",
            $"{ItemType.GunAK}",
            $"{ItemType.KeycardChaosInsurgency}",
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.ChaosRifleman,
                    Chance = 100,
                },
            },
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato762, 90
            },
        };

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new Replicator
            {
                Name = "Replicator [Active]",
                Description = "Create decoy and make recon",
            },
        };
        
        public override string AbilityUsage { get; set; } = "Use your Replicator ability with your server keybind (set it in ESC > Settings > Server Settings).";
    }
}