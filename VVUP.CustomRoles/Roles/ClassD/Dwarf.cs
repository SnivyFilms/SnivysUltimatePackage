using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.ClassD
{
    public class Dwarf : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override uint Id { get; set; } = 60;
        public override int MaxHealth { get; set; } = 60;
        public override string Name { get; set; } = "<color=#FF8E00>Dwarf Class-D</color>";
        public override string Description { get; set; } = "A smaller Class-D, having less health but being harder to hit, infinite stamina, but reduced item access.";
        public override string CustomInfo { get; set; } = "Dwarf Class-D";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility
            {
                Name = "Dwarf [Passive]",
                Description = "Makes you small as a dwarf",
                ScaleForPlayers = new Vector3(0.75f, 0.75f, 0.75f),
            },
            new InfiniteStamina
            {
                Name = "Infinite Stamina [Passive]",
                Description = "Gives you infinite stamina",
            },
            new RestrictedItems
            {
                Name = "Restricted Items [Passive]",
                Description = "Restricts certain items from being used",
                RestrictedItemList = new List<ItemType>
                {
                    ItemType.ArmorCombat,
                    ItemType.ArmorHeavy,
                    ItemType.GunAK,
                    ItemType.GunE11SR,
                    ItemType.GunFRMG0,
                    ItemType.GunLogicer,
                    ItemType.GunShotgun,
                    ItemType.GunSCP127,
                    ItemType.MicroHID,
                    ItemType.ParticleDisruptor,
                    ItemType.SCP1344,
                    ItemType.SCP268,
                    ItemType.AntiSCP207,
                },
                RestrictUsingItems = true,
                RestrictPickingUpItems = true,
                RestrictDroppingItems = false,
            },
            new CustomRoleEscape
            {
                Name = "Custom Role Escape [Passive]",
                Description = "If you escape as a Dwarf Class-D, you will get a custom role.",
                EscapeToRegularRole = false,
                CuffedEscapeCustomRole = "<color=#0096FF>MTF Wisp</color>",
                UncuffedEscapeCustomRole = "<color=#008f1e>Telepathic Chaos</color>",
                AllowCuffedCustomRoleChange = true,
                AllowUncuffedCustomRoleChange = true,
                SaveInventory = false,
                UseOnSpawnCuffedEscape = false,
                UseOnSpawnUncuffedEscape = true,
            },
        };
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
        
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
    }
}