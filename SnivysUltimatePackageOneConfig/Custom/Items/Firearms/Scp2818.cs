﻿using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;

namespace SnivysUltimatePackageOneConfig.Custom.Items.Firearms
{
    [CustomItem(ItemType.GunE11SR)]
    public class Scp2818 : CustomWeapon
    {
        public override uint Id { get; set; } = 33;
        public override string Name { get; set; } = "<color=#FF0000>SCP-2818</color>";

        public override string Description { get; set; } =
            "When this weapon is fired, it uses the biomass of the shooter as the bullet.";

        public override float Weight { get; set; } = 4;
        public override float Damage { get; set; } = 1000;
        public override byte ClipSize { get; set; } = 1;

        [Description("Whether or not the weapon should despawn itself after it's been used.")]
        public bool DespawnAfterUse { get; set; } = false;

        public string DeathReasonUser { get; set; } = "Vaporized by becoming a bullet";
        public string DeathReasonTarget { get; set; } = "Vaporized by a human bullet";

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
            },
        };

        protected override void OnShot(ShotEventArgs ev)
        {
            if (ev.Target == null)
            {
                Log.Debug($"VVUP Custom Items: SCP-2818, {ev.Player.Nickname} fired and missed a target, teleporting them to bullet impact location ({ev.Position}");
                ev.Player.Position = ev.Position;
            }
            else
            {
                Log.Debug($"VVUP Custom Items: SCP-2818, {ev.Player.Nickname} shot and hit {ev.Target.Nickname}, running hit code");
                ev.CanHurt = false;
                ev.Player.Position = ev.Target.Position;
                if (ev.Target.Health <= Damage)
                {
                    Log.Debug($"VVUP Custom Items: SCP-2818, {ev.Target.Nickname} has {ev.Target.Health} but damage is set to {Damage}. Killing {ev.Target.Nickname}");
                    ev.Target.Kill(DeathReasonTarget);
                }
                else
                {
                    Log.Debug($"VVUP Custom Items: SCP-2818, {ev.Target.Nickname} has {ev.Target.Health} which is higher than {Damage}, dealing {Damage} to {ev.Target.Nickname}");
                    ev.Target.Hurt(Damage);
                }
            }

            if (DespawnAfterUse)
            {
                Log.Debug($"VVUP Custom Items: SCP-2818, Despawn After Use is true, removing SCP-2818 from {ev.Player.Nickname}'s inventory");
                ev.Player.RemoveItem(ev.Item);
            }
            Log.Debug($"VVUP Custom Items: SCP-2818, Killing {ev.Player.Nickname}");
            ev.Player.Kill(DeathReasonUser);
        }
    }
}