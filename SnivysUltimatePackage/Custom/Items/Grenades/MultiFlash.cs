﻿using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;
using Item = Exiled.API.Features.Items.Item;
using Log = Exiled.API.Features.Log;
using Random = System.Random;
using Server = Exiled.API.Features.Server;

namespace SnivysUltimatePackage.Custom.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class MultiFlash : CustomGrenade
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override uint Id { get; set; } = 42;
        public override string Name { get; set; } = "<color=#6600CC>Echo Flare</color>";

        public override string Description { get; set; } =
            "When this flash goes off, it spawns other flashes";

        public override float Weight { get; set; } = 1.75f;
        [CanBeNull]
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideLczArmory,
                },

                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideHczArmory,
                },

                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.Inside049Armory,
                },

                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideSurfaceNuke,
                },
                new ()
                {
                    Chance = 15,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 2.5f;
        public int FlashGrenadeCount { get; set; } = 5;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            Log.Debug("VVUP Custom Items: MultiFlash, initial grenade detonated, running methods");
            Timing.CallDelayed(0.1f, () =>
            {
                Log.Debug("VVUP Custom Items: MultiFlash, Spawning a small grenade to scatter the other grenades");
                ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeFlash);
                Log.Debug($"VVUP Custom Items: MultiFlash, setting grenades ownership from the server to {ev.Player.Nickname}");
                grenade.ChangeItemOwner(null, ev.Player);
                grenade.SpawnActive(ev.Position, ev.Player);
                grenade.FuseTime = FuseTime;
                for (int i = 0; i <= FlashGrenadeCount; i++)
                {
                    Timing.CallDelayed(FuseTime * i, () =>
                    {
                        Log.Debug(
                            $"VVUP Custom Items: MultiFlash, spawning {FlashGrenadeCount - i} more grenades at {ev.Position}");
                        grenade.ChangeItemOwner(null, ev.Player);
                        grenade.SpawnActive(ev.Position, owner: ev.Player);
                    });
                }
            });
        }
    }
}