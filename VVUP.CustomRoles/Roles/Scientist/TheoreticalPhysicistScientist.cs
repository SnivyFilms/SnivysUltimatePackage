using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Scientist
{
    public class TheoreticalPhysicistScientist : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public override uint Id { get; set; } = 49;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;
        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;
        
        public override int MaxHealth { get; set; } = 100;
        
        public override string Name { get; set; } = "<color=#FFFF7C>Theoretical Physicist Scientist</color>";

        public override string Description { get; set; } =
            "A Scientist that believes that the pocket dimension has a lot more to offer than what it is otherwise normally thought of.";
        
        public override string CustomInfo { get; set; } = "Theoretical Physicist Scientist";
        
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardScientist.ToString(),
            ItemType.Painkillers.ToString(),
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new PocketDimensionEscapeChance()
            {
                Name = "Pocket Dimension Escape Chance [Passive]",
                Description = "Makes it so the player can escape the pocket dimension twice, guaranteed.",
                EscapeChance = 100,
                AmountOfAllowedEscapes = 2,
                CustomDeathReason = "You have hit your guaranteed escape limit from the pocket dimension.",
                RemoveTraumatizedOnEscape = true,
            },
            new CustomRoleEscape
            {
                Name = "Custom Role Escape [Passive]",
                Description = "If you escape as this custom role, this will make sure you get your inventory.",
                EscapeToRegularRole = true,
                RegularRole = RoleTypeId.NtfSpecialist,
                AllowCuffedCustomRoleChange = false,
                AllowUncuffedCustomRoleChange = false,
                SaveInventory = true
            },
        };
        
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
    }
}