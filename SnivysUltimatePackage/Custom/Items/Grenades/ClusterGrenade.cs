using System.Collections.Generic;
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
    [CustomItem(ItemType.GrenadeHE)]
    public class ClusterGrenade : CustomGrenade
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeHE;
        public override uint Id { get; set; } = 36;
        public override string Name { get; set; } = "<color=#FF0000>Cluster Grenade</color>";

        public override string Description { get; set; } =
            "When this grenade explodes, it spawns extra grenades near by";

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
        public override float FuseTime { get; set; } = 5;
        [Description("How long is the additional grenades fuse times")]
        public float ClusterGrenadeFuseTime { get; set; } = 1.5f;
        public int ClusterGrenadeCount { get; set; } = 5;
        [Description("Enables a random spread of the cluster grenades, if its off it will spawn all of them on top of the detonation point")]
        public bool ClusterGrenadeRandomSpread { get; set; } = true;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            Log.Debug("VVUP Custom Items: Cluster Grenade, initial grenade detonated, running methods");
            Log.Debug("VVUP Custom Items: Cluster Grenade, running spawning cluster grenades");
            Timing.CallDelayed(0.1f, () =>
            {
                Log.Debug("VVUP Custom Items: Cluster Grenade, Spawning a small grenade to scatter the other grenades");
                ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                grenade.FuseTime = 0.25f;
                grenade.ScpDamageMultiplier = 0.5f;
                Log.Debug($"VVUP Custom Items: Cluster Grenade, setting grenades ownership from the server to {ev.Player.Nickname}");
                grenade.ChangeItemOwner(null, ev.Player);
                grenade.SpawnActive(ev.Position, ev.Player);
                grenade.FuseTime = ClusterGrenadeFuseTime;
                grenade.ScpDamageMultiplier = 3;
                for (int i = 0; i <= ClusterGrenadeCount; i++)
                {
                    Log.Debug(
                        $"VVUP Custom Items: Cluster Grenade, spawning {ClusterGrenadeCount - i} more grenades at {ev.Position}");
                    grenade.ChangeItemOwner(null, ev.Player);
                    if (ClusterGrenadeRandomSpread)
                        grenade.SpawnActive(GrenadeOffset(ev.Position), owner: ev.Player);
                    else
                        grenade.SpawnActive(ev.Position, owner: ev.Player);
                }
            });
        }

        private Vector3 GrenadeOffset(Vector3 position)
        {
            Random random = new Random();
            float x = position.x - 1 + ((float)random.NextDouble() * random.Next(0, 3));
            float y = position.y;
            float z = position.z - 1 + ((float)random.NextDouble() * random.Next(0, 3));
            return new Vector3(x, y, z);
        }
    }
}