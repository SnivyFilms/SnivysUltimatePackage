﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;

namespace SnivysUltimatePackage.Custom.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class NerveAgentGrenade : CustomGrenade
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override uint Id { get; set; } = 22;
        public override string Name { get; set; } = "<color=#FF0000>Nerve Agent Grenade</color>";
        public override string Description { get; set; } = "Deploys Nerve Agent";
        public override float Weight { get; set; } = 1f;
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3f;
        public float NerveAgentDuration { get; set; } = 25f;
        public float NerveAgentRadius { get; set; } = 5f;
        public float NerveAgentPoisonDuration { get; set; } = 30f;
        public float NerveAgentImmediateDamage { get; set; } = 1f;
        private Vector3 grenadePosition;
        private Pickup pickup;
        private CoroutineHandle nerveAgentHandle;
        [CanBeNull]
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideLczArmory,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideSurfaceNuke,
                },
            }
        };

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            Log.Debug("VVUP Custom Items: Nerve Agent, Starting Routine");
            grenadePosition = ev.Position;
            Scp244 scp244 = (Scp244)Item.Create(ItemType.SCP244a);
            scp244.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            scp244.Primed = true;
            scp244.MaxDiameter = 0f;
            pickup = scp244.CreatePickup(grenadePosition);
            nerveAgentHandle = Timing.RunCoroutine(NerveAgentCoroutine());
        }

        public IEnumerator<float> NerveAgentCoroutine()
        {
            float timeRemaining = NerveAgentDuration;
            for (;;)
            {
                if (timeRemaining <= 0 || Round.IsEnded || Round.IsLobby)
                {
                    pickup.Position += Vector3.down;
                    pickup.Position += Vector3.down;
                    pickup.Position += Vector3.down;
                    pickup.Position += Vector3.down;
                    pickup.Position += Vector3.down;
                    Timing.CallDelayed(5, () =>
                    {
                        Log.Debug("VVUP Custom Items: Nerve Agent, Ending Routine");
                        pickup.Destroy();
                    });
                    grenadePosition = Vector3.zero;
                    Timing.KillCoroutines(nerveAgentHandle);
                    yield break;
                }
                foreach (PlayerAPI player in PlayerAPI.List)
                {
                    if(Vector3.Distance(player.Position, grenadePosition) <= NerveAgentRadius)
                    {
                        player.EnableEffect(EffectType.Poisoned, NerveAgentPoisonDuration);
                        player.Hurt(NerveAgentImmediateDamage, DamageType.Poison);
                    }
                }

                timeRemaining -= 0.5f;
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}