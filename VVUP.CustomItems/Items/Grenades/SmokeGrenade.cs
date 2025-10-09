using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class SmokeGrenade : CustomGrenade
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override uint Id { get; set; } = 20;
        public override string Name { get; set; } = "<color=#6600CC>Obscurus Veil-5</color>";

        public override string Description { get; set; } =
            "This flash is a smoke grenade, when detonated, a smoke cloud will be deployed";

        public override float Weight { get; set; } = 1.15f;
        
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3f;
        public bool RemoveSmoke { get; set; } = true;
        [Description("If RemoveSmoke is true, how long does it take before the smoke will be removed")]
        public float FogTime { get; set; } = 10;
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGr18,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideSurfaceNuke,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideLczArmory,
                },
            },
        };
        
        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            Vector3 savedGrenadePosition = ev.Position;
            Scp244 scp244 = (Scp244) Item.Create(ItemType.SCP244a);
            Pickup pickup = null;
            scp244.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            scp244.Primed = true;
            scp244.MaxDiameter = 0.0f;
            pickup = scp244.CreatePickup(savedGrenadePosition);
            if (RemoveSmoke)
            {
                Timing.CallDelayed(FogTime, () =>
                {
                    pickup.Position += Vector3.down * 10;
                    
                    Timing.CallDelayed(10, () =>
                    {
                        pickup.Destroy(); 
                    });
                });
            }
        }
    }
}