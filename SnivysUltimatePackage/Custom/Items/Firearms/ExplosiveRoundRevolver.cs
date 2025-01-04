﻿using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using YamlDotNet.Serialization;
using Player = Exiled.Events.Handlers.Player;

namespace SnivysUltimatePackage.Custom.Items.Firearms
{
    [CustomItem(ItemType.GunRevolver)]
    public class ExplosiveRoundRevolver : CustomWeapon
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GunRevolver;
        public override uint Id { get; set; } = 21;
        public override string Name { get; set; } = "<color=#FF0000>Explosive Round Revolver</color>";
        public override string Description { get; set; } = "This revolver fires explosive rounds.";
        public override float Weight { get; set; } = 1f;
        [CanBeNull]
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside096,
                },
            }
        };

        public override float Damage { get; set; } = 0;
        public override byte ClipSize { get; set; } = 2;
        public float FuseTime { get; set; } = 2.5f;
        public float ScpGrenadeDamageMultiplier { get; set; } = .5f;

        protected override void SubscribeEvents()
        {
            Player.Shot += OnShot;
        }

        protected override void UnsubscribeEvents()
        {
            Player.Shot -= OnShot;
        }

        private void OnShot(ShotEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Log.Debug($"VVUP Custom Items: Explosive Round Revolver, spawning grenade at {ev.Position}");
            ev.CanHurt = false;
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = FuseTime;
            grenade.ScpDamageMultiplier = ScpGrenadeDamageMultiplier;
            grenade.SpawnActive(ev.Position);
        }
    }
}