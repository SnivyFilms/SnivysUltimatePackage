using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Scientist
{
    public class Biochemist : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;

        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;

        public override uint Id { get; set; } = 32;

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#FFFF7C>Biochemist</color>";

        public override string Description { get; set; } =
            "A scientist that specializes in biological altering. You can passively heal other people and explode on death.";

        public override string CustomInfo { get; set; } = "Biochemist";

        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Medkit.ToString(),
            ItemType.Adrenaline.ToString(),
            ItemType.Radio.ToString(),
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new CustomRoleEscape
            {
                Name = "Custom Role Escape [Passive]",
                Description = "If you escape as a Biochemist, you''re guaranteed a custom role.",
                UncuffedEscapeCustomRole = "<color=#0096FF>MTF Demolitionist</color>",
                CuffedEscapeCustomRole = "<color=#008f1e>Explosive Chaos</color>",
                AllowUncuffedCustomRoleChange = true,
                AllowCuffedCustomRoleChange = true,
                SaveInventory = true
            },
            new Martyrdom
            {
                Name = "Explode on Death [Passive]",
                Description = "You explode on death",
                ExplosiveFuse = 0.1f
            },
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
    }
}