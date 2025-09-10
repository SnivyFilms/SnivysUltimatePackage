using System.Collections.Generic;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class ProxyBang : CustomGrenade
    {
        [YamlIgnore] public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override uint Id { get; set; } = 43;
        public override string Name { get; set; } = "<color=#6600CC>Pathfinder Grenade</color>";

        public override string Description { get; set; } =
            "When detonates, it shows lines to all players in the area.";

        public override float Weight { get; set; } = 1.75f;
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideHidChamber,
                },
            },
        };

        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 10;
        public float Range { get; set; } = 10;
        public float LineVisibleTime { get; set; } = 5;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            foreach (PlayerAPI player in PlayerAPI.List)
            {
                if (Vector3.Distance(ev.Position, player.Position) <= Range)
                {
                    var color = GetTeamColor(player);
                    var lineColor = new Color(color.red, color.green, color.blue);
                    var direction = player.Position - ev.Position;
                    var distance = direction.magnitude;
                    var scale = new Vector3(0.1f, distance * 0.5f, 0.1f);
                    var laserPos = ev.Position + direction * 0.5f;
                    var rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
                    Log.Debug($"VVUP Custom Items: Proxy Bang, Laser Info: Position: {laserPos}, Rotation: {rotation.eulerAngles}, Color: {lineColor}");
                    var laser = Primitive.Create(PrimitiveType.Cylinder, PrimitiveFlags.Visible, laserPos, rotation.eulerAngles,
                        scale, true, lineColor);
                    Timing.CallDelayed(LineVisibleTime, laser.Destroy);
                }
            }
        }

        private (float red, float green, float blue) GetTeamColor(PlayerAPI player)
        {
            float red;
            float green;
            float blue;
            
            switch (player.Role.Side)
            {
                case Side.Mtf:
                    red = 0;
                    green = 0.39f;
                    blue = 1;
                    break;
                case Side.ChaosInsurgency:
                    red = 0;
                    green = 0.51f;
                    blue = 0;
                    break;
                case Side.Scp:
                    red = 0.59f;
                    green = 0;
                    blue = 0;
                    break;
                default:
                    red = 1;
                    green = 0.41f;
                    blue = 0.71f;
                    break;
            }
            
            return (red, green, blue);
        }
    }
}