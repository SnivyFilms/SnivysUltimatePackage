using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;
using Item = Exiled.API.Features.Items.Item;
using Server = Exiled.API.Features.Server;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeHE)]
    public class GuardianGrenade : CustomGrenade
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeHE;

        public override uint Id { get; set; } = 55;
        public override string Name { get; set; } = "<color=#FF0000>Guardian Grenade</color>";
        public override string Description { get; set; } = "A grenade that detonates when people get close";
        public override float Weight { get; set; } = 0.75f;
        public override bool ExplodeOnCollision { get; set; } = false;

        [Description("Fuse time before the grenade removes itself if no one comes close.")]
        public override float FuseTime { get; set; } = 9999;
        [Description("The radius in which the grenade will detonate when a player enters.")]
        public float TriggerRadius { get; set; } = 3.5f;
        [Description("The delay before the grenade becomes active and can detonate.")]
        public float ActivationDelay { get; set; } = 2.0f;

        [Description("Delay after a player enters the radius")]
        public float DetonationDelay { get; set; } = 2.0f;

        [Description("Should the grenade ignore the thrower's teammates? (This doesnt mean friendly fire on/off)")]
        public bool IgnoreTeammates { get; set; } = false;

        [Description("Should the grenade be associated with the server or the player?")]
        public bool AssociateAsServer { get; set; } = false;
        [Description("How often should the grenade check for players in range? (in seconds)")]
        public float CheckInterval { get; set; } = 0.25f;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideLczArmory
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHczArmory
                }
            },
        };
        
        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            Timing.CallDelayed(ActivationDelay, () =>
            {
                Timing.RunCoroutine(MonitorProximity(ev));
            });
            base.OnThrownProjectile(ev);
        }
        
        private IEnumerator<float> MonitorProximity(ThrownProjectileEventArgs ev)
        {
            while (ev.Projectile != null)
            {
                foreach (var player in Exiled.API.Features.Player.List)
                {
                    if (player == null || (IgnoreTeammates && player.Role.Team == ev.Player.Role.Team))
                        continue;
                
                    if (Vector3.Distance(player.Position, ev.Projectile.Position) <= TriggerRadius)
                    {
                        yield return Timing.WaitForSeconds(DetonationDelay);

                        if (ev.Projectile != null)
                        {
                            ev.Projectile.Destroy();
                            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(Type);
                            grenade.FuseTime = DetonationDelay;
                            grenade.SpawnActive(ev.Projectile.Position, owner: AssociateAsServer ? Server.Host : ev.Player);
                        }

                        yield break;
                    }
                }
        
                yield return Timing.WaitForSeconds(CheckInterval);
            }
        }
    }
}