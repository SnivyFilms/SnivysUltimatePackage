using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Chaos
{
    public class Nightfall : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 5;

        public StartTeam StartTeam { get; set; } = StartTeam.Chaos;

        public override uint Id { get; set; } = 35;

        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosRifleman;

        public override int MaxHealth { get; set; } = 200;

        public override string Name { get; set; } = "<color=#008f1e>Nightfall</color>";

        public override string Description { get; set; } =
            "A Chaos Insurgent that is special. You figure out the rest.";

        public override string CustomInfo { get; set; } = "Nightfall";

        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.GunRevolver}",
            $"{ItemType.KeycardO5}",
            $"{ItemType.SCP500}",
            $"{ItemType.ArmorCombat}",
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Ammo44Cal, 20
            },
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new RestrictedItems()
            {
                Name = "Nightfall Ability [Passive]",
                Description = "Yeah I am not going to tell you what this is",
                RestrictedItemList =
                {
                    ItemType.Medkit,
                    ItemType.Painkillers,
                    ItemType.SCP500,
                    ItemType.KeycardO5,
                    ItemType.GunE11SR,
                    ItemType.GunCrossvec,
                    ItemType.GunFSP9,
                    ItemType.GunLogicer,
                    ItemType.GunAK,
                    ItemType.GunShotgun
                },
                RestrictUsingItems = false,
                RestrictDroppingItems = true,
                RestrictPickingUpItems = true
            }
        };
        
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}