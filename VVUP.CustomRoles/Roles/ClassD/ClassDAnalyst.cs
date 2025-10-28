using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.ClassD
{
    public class ClassDAnalyst : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override uint Id { get; set; } = 51;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#FF8E00>Class-D Analyst</color>";

        public override string Description { get; set; } =
            "A Class-D with a knack for analyzing and gathering information.";

        public override string CustomInfo { get; set; } = "Class-D Analyst";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new CustomRoleEscape
            {
                Name = "Custom Role Escape [Passive]",
                Description = "If you escape as a Class-D Analyst, you''re guaranteed a custom role.",
                UncuffedEscapeCustomRole = "<color=#008f1e>Telepathic Chaos</color>",
                CuffedEscapeCustomRole = "<color=#0096FF>MTF Wisp</color>",
                AllowUncuffedCustomRoleChange = true,
                AllowCuffedCustomRoleChange = true,
                SaveInventory = true
            },
            new Detect
            {
                Name = "Detect [Active]",
                Description = "Detects targets for you, a Class-D.",
                Duration = 0,
                Cooldown = 120,
                DetectRange = 30,
                ShowMissingRoles = true,
                RoleNames = new Dictionary<RoleTypeId, string>()
                {
                    { RoleTypeId.Scp049, "SCP-049" },
                    { RoleTypeId.Scp0492, "SCP-049-2" },
                    { RoleTypeId.Scp079, "SCP-079" },
                    { RoleTypeId.Scp096, "SCP-096" },
                    { RoleTypeId.Scp106, "SCP-106" },
                    { RoleTypeId.Scp173, "SCP-173" },
                    { RoleTypeId.Scp939, "SCP-939" },
                },
            },
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
        
        public override string AbilityUsage { get; set; } = "Use your Noclip Button [Left Alt] to swap abilities and to activate. Tap Twice to Swap. Tap Once to activate.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}