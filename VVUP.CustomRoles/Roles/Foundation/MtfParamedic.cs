using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Foundation
{
    public class MtfParamedic : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;
        public override uint Id { get; set; } = 50;
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfSpecialist;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#0096FF>MTF Paramedic</color>";

        public override string Description { get; set; } =
            "A paramedic that has extra medical equipment";
        public override string CustomInfo { get; set; } = "MTF Paramedic";
        
        public override List<string> Inventory { get; set; } = new()
        {
            "<color=#0096FF>Phantom Pulse</color>",
            ItemType.Medkit.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorCombat.ToString()
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato9, 80
            },
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

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            /*new RevivingMist
            {
                Name = "Reviving Mist [Active]",
                Description = "Allows you to revive teammates",
                Duration = 1,
                Cooldown = 180,
                ReviveRadius = 5,
                ReviveTimeWindow = 30f,
                ReviveHealthPercent = 30f,
                ReviveMessage = "You have been revived by a Paramedic!",
                ReviveMessageTime = 5,
                ReviveTeammatesOnly = true,
            },*/
            new HealingMist
            {
                Name = "Healing Mist [Active]",
                Description =
                    "Activates a short-term spray of chemicals which will heal and protect allies for a short duration.",
                Duration = 15,
                Cooldown = 180,
                HealAmount = 6,
                ProtectionAmount = 45,
            },
        };
        
        public override string AbilityUsage { get; set; } = "Use your Noclip Button [Left Alt] to swap abilities and to activate. Tap Twice to Swap. Tap Once to activate.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}