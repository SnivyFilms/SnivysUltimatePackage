﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Roles.Chaos
{
    public class CiPhantom : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public override uint Id { get; set; } = 44;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#008f1e>Chaos Phantom</color>";
        public override string Description { get; set; } = "A Chaos Insurgent specialized in espionage";
        public override string CustomInfo { get; set; } = "Chaos Phantom";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosConscript;

        public StartTeam StartTeam { get; set; } = StartTeam.Guard;

        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new RoleSpawnPoint
                {
                    Role = RoleTypeId.FacilityGuard,
                    Chance = 100,
                },
            },
        };

        public override List<string> Inventory { get; set; } = new List<string>
        {
            "<color=#6600CC>PB-42</color>",
            "<color=#6600CC>Obscurus Veil-5</color>",
            $"{ItemType.GunCrossvec}",
            $"{ItemType.Medkit}",
            $"{ItemType.KeycardChaosInsurgency}",
            $"{ItemType.Adrenaline}",
            $"{ItemType.ArmorCombat}",
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato9, 80
            },
        };
        
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ActiveCamo
            {
                Name = "Active Camo [Active]",
                Description = "Activates camo, will reapply when doing most actions",
            },
        };
        
        public override string AbilityUsage { get; set; } = "Use your Noclip Button [Left Alt] to swap abilities and to activate. Tap Twice to Swap. Tap Once to activate.";

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            base.SubscribeEvents();
        }
        
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            base.UnsubscribeEvents();
        }

        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (Check(ev.Player) && ev.Pickup.Type == ItemType.SCP268)
                ev.IsAllowed = false;
        }
    }
}