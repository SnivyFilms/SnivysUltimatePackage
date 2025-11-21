using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using InventorySystem.Items.Usables.Scp330;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Chaos
{
    public class JuggernautChaos : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 10;

        public StartTeam StartTeam { get; set; } = StartTeam.Chaos;

        public override uint Id { get; set; } = 39;

        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosConscript;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#008f1e>Explosive Chaos</color>";

        public override string Description { get; set; } = "A Chaos Insurgent specializing in explosives";

        public override string CustomInfo { get; set; } = "Explosive Chaos";

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.ChaosConscript,
                    Chance = 100,
                },
            },
        };

        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.KeycardMTFPrivate}",
            $"{ItemType.GrenadeHE}",
            $"{ItemType.GrenadeHE}",
            $"{ItemType.GrenadeFlash}",
            $"{ItemType.ArmorHeavy}",
            "<color=#FF0000>ADATS</color>",
            "<color=#6600CC>Obscurus Veil-5</color>",
        };
        
        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Ammo44Cal, 8
            },
        };
        
        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new GivingCandyAbility
            {
                Name = "Giving Candy Ability [Passive]",
                Description = "Gives you a pink candy when you spawn in.",
                StartingCandy =
                {
                    CandyKindID.Pink,
                },
            },
            new CustomRoleEscape
            {
                Name = "Custom Role Escape [Passive]",
                Description =
                    "If you get detained as a Chaos, if you get escorted out you''re guaranteed to be a MTF Demolitionist",
                UncuffedEscapeCustomRole = "",
                CuffedEscapeCustomRole = "<color=#0096FF>MTF Demolitionist</color>",
                AllowUncuffedCustomRoleChange = false,
                AllowCuffedCustomRoleChange = true,
                SaveInventory = true,
            },
        };
        
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}