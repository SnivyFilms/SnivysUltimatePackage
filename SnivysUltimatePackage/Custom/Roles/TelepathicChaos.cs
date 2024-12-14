﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using SnivysUltimatePackage.API;
using SnivysUltimatePackage.Custom.Abilities;

namespace SnivysUltimatePackage.Custom.Roles;

[CustomRole(RoleTypeId.ChaosConscript)]
public class TelepathicChaos : CustomRole, ICustomRole
{
    public int Chance { get; set; } = 10;

    public StartTeam StartTeam { get; set; } = StartTeam.Chaos;

    public override uint Id { get; set; } = 38;

    public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosRifleman;

    public override int MaxHealth { get; set; } = 100;

    public override string Name { get; set; } = "<color=#008f1e>Telepathic Chaos</color>";

    public override string Description { get; set; } = "You have the ability to FEEL enemies of the Chaos Insergency near you";

    public override string CustomInfo { get; set; } = "Telepathic Chaos";

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
        $"{ItemType.GunA7}",
        $"{ItemType.KeycardChaosInsurgency}",
        $"{ItemType.Medkit}",
        $"{ItemType.ArmorCombat}",
        $"{ItemType.Painkillers}",
        "<color=#6600CC>Obscurus Veil-5</color>",
    };

    public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
    {
        {
            AmmoType.Nato762, 120
        },
    };
    public override List<CustomAbility>? CustomAbilities { get; set; } = new()
    {
        new Detect
        {
            Name = "Detect [Active]",
            Description = "Detects targets of the Chaos Insurgency.",
            Duration = 0,
            Cooldown = 120,
            DetectRange = 30,
            RoleNames =
            {
                {RoleTypeId.Scientist, "Sci"},
                {RoleTypeId.NtfCaptain, "MTF Cap"},
                {RoleTypeId.NtfPrivate, "MTF Pri"},
                {RoleTypeId.NtfSergeant, "MTF Ser"},
                {RoleTypeId.NtfSpecialist, "MTF Spe"},
                {RoleTypeId.FacilityGuard, "Guard"},
                {RoleTypeId.Scp049, "049"},
                {RoleTypeId.Scp0492, "049-2"},
                {RoleTypeId.Scp079, "079"},
                {RoleTypeId.Scp096, "096"},
                {RoleTypeId.Scp106, "106"},
                {RoleTypeId.Scp173, "173"},
                {RoleTypeId.Scp939, "939"},
                {RoleTypeId.Scp3114, "3114"},
                {RoleTypeId.Tutorial, "SH"},
            },
        },
    };
}