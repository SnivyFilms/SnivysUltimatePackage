using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;
using PlayerLab = LabApi.Features.Wrappers.Player;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeHE)]
    public class LowGravityGrenade : CustomGrenade
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeHE;

        public override uint Id { get; set; } = 51;
        public override string Name { get; set; } = "<color=#6600CC>Lunar Lob</color>";

        public override string Description { get; set; } =
            "When detonated, it causes a low gravity effect to nearby players";

        public override float Weight { get; set; } = 1;
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 2;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideLczArmory,
                },
                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideGr18Glass,
                },
                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideGateA,
                },
                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideGateB,
                },
            }
        };
        
        public Vector3 LowGravity { get; set; } = new(0, -12.60f, 0);
        public float Duration { get; set; } = 15f;
        public float Range { get; set; } = 15f;
        private Dictionary<Player, Vector3> _effectedPlayers = new();

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;

            foreach (Player player in Player.List)
            {
                if (Vector3.Distance(ev.Position, player.Position) <= Range)
                {
                    Vector3 previousGravity = PlayerLab.Get(player.NetworkIdentity)!.Gravity;
                    _effectedPlayers[player] = previousGravity;
                    PlayerLab.Get(player.NetworkIdentity)!.Gravity = LowGravity;
                    Timing.CallDelayed(Duration, () =>
                    {
                        PlayerLab.Get(ev.Player.NetworkIdentity)!.Gravity = _effectedPlayers[ev.Player];
                        _effectedPlayers.Remove(ev.Player);
                    });
                }
            }
        }
    }
}