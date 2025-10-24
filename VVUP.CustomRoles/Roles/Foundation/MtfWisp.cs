using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.Base;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Foundation
{
    public class MtfWisp : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;

        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;

        public override uint Id { get; set; } = 41;

        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfSpecialist;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#0096FF>MTF Wisp</color>";

        public override string Description { get; set; } =
            "A MTF Specialist that has the ability to go through doors but at a cost of reduced stamina.";

        public override string CustomInfo { get; set; } = "MTF Wisp";

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.GunCrossvec.ToString(),
            ItemType.GunRevolver.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorCombat.ToString()
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato9, 80
            },
            {
                AmmoType.Ammo44Cal, 12
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

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new RestrictedItems
            {
                Name = "Restricted Items [Passive]",
                Description = "Handles restricted items",
                RestrictedItemList =
                {
                    ItemType.Adrenaline,
                    ItemType.SCP500
                },
                RestrictUsingItems = true,
                RestrictPickingUpItems = true,
                RestrictDroppingItems = false
            },
            new EffectEnabler
            {
                Name = "Wisp [Passive]",
                Description = "Enables walking through doors, Fog Control, Reduced Sprint",
                EffectsToApply = new List<ApplyEffects>()
                {
                    new ApplyEffects()
                    {
                        EffectType = EffectType.Exhausted
                    },
                    new ApplyEffects()
                    {
                        EffectType = EffectType.Ghostly,
                        Intensity = 1,
                    },
                    new ApplyEffects()
                    {
                        EffectType = EffectType.FogControl,
                        Intensity = 1,
                    }
                },
            },
        };
        
        public override string AbilityUsage { get; set; } = "You have passive abilities. This does not require button activation";
    }
}