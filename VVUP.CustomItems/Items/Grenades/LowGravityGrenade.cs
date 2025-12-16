using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using PlayerLab = LabApi.Features.Wrappers.Player;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeHE)]
    public class LowGravityGrenade : CustomGrenade, ICustomItemGlow
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
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            base.UnsubscribeEvents();
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;

            foreach (Player player in Player.List.Where(p => Vector3.Distance(ev.Position, p.Position) <= Range))
            {
                Log.Debug($"VVUP Custom Items: Low Gravity Grenade, Applying low gravity {LowGravity} to {player.Nickname}. Waiting {Duration} seconds to revert.");
                Vector3 previousGravity = PlayerLab.Get(player.NetworkIdentity)!.Gravity;
                _effectedPlayers[player] = previousGravity;
                PlayerLab.Get(player.NetworkIdentity)!.Gravity = LowGravity;
                Timing.CallDelayed(Duration, () =>
                {
                    Log.Debug($"VVUP Custom Items: Low Gravity Grenade, Reverting gravity for {player.Nickname} to {previousGravity}");
                    if (!_effectedPlayers.ContainsKey(player))
                        return;
                    PlayerLab.Get(player.NetworkIdentity)!.Gravity = _effectedPlayers[player];
                    _effectedPlayers.Remove(player);
                });
            }
        }

        protected override void OnWaitingForPlayers()
        {
            _effectedPlayers.Clear();
            base.OnWaitingForPlayers();
        }
        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (_effectedPlayers.ContainsKey(ev.Player))
            {
                Log.Debug($"VVUP Custom Items: Low Gravity Grenade, Reverting gravity for {ev.Player.Nickname} to {_effectedPlayers[ev.Player]} due to role change");
                PlayerLab.Get(ev.Player.NetworkIdentity)!.Gravity = _effectedPlayers[ev.Player];
                _effectedPlayers.Remove(ev.Player);
            }
        }
    }
}