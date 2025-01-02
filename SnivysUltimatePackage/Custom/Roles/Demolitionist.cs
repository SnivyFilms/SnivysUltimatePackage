﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using SnivysUltimatePackage.API;

namespace SnivysUltimatePackage.Custom.Roles
{
    public class Demolitionist : CustomRole
    {
        public int Chance { get; set; } = 15;

        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;
        public override uint Id { get; set; } = 47;
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfSpecialist;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#0096FF>MTF Demolitionist</color>";
        public override string Description { get; set; } = "A MTF Member that is specialized in explosives";
        public override string CustomInfo { get; set; } = "MTF Demolitionist";
        
        public override List<string> Inventory { get; set; } = new()
        {
            "<color=#FF0000>Explosive Round Revolver</color>",
            "<color=#FF0000>Explosive Resistant Armor</color>",
            "<color=#FF0000>C4</color>",
            "<color=#FF0000>C4</color>",
            "<color=#FF0000>C4</color>",
            ItemType.GrenadeHE.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.Radio.ToString(),
        };
        
        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Ammo44Cal, 8
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
    }
}