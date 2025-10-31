using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using VVUP.Base.API;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunE11SR)]
    public class Scp2818 : CustomWeapon, ICustomItemGlow
    {
        public override uint Id { get; set; } = 33;
        public override string Name { get; set; } = "<color=#FF0000>SCP-2818</color>";

        public override string Description { get; set; } =
            "When this weapon is fired, it uses the biomass of the shooter as the bullet.";

        public override float Weight { get; set; } = 4;
        public override float Damage { get; set; } = 1000;
        public override byte ClipSize { get; set; } = 1;

        [Description("Whether or not the weapon should despawn itself after it was shot and did not miss.")]
        public bool DespawnAfterUse { get; set; } = true;
        [Description("Whether or not the weapon should despawn itself after it was shot and missed.")]
        public bool DespawnAfterMiss { get; set; } = true;
        [Description("Should the player instantly die after missing a shot with this weapon?")]
        public bool KillAfterMiss { get; set; } = true;
        [Description("Should it check if wherever the player shot was the floor? Note: Disabling this will allow players to get into spots they shouldn't be able to be in.")]
        public bool UseRaycastPosition { get; set; } = true;

        public string DeathReasonUser { get; set; } = "Vaporized by becoming a bullet.";
        public string DeathReasonTarget { get; set; } = "Vaporized by a human bullet.";

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
                new ()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 127);
        public float GlowRange { get; set; } = 0.25f;

        protected override void OnShot(ShotEventArgs ev)
        {
            bool miss = false;
            if (ev.Target == null)
            {
                miss = true;
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



            Timing.CallDelayed(0.1f, () =>
            {
                if (miss)
                {
                    Log.Debug($"VVUP Custom Items: SCP-2818, {ev.Player.Nickname} fired and missed a target, teleporting them to bullet impact location ({ev.RaycastHit.point}");

                    if (DespawnAfterMiss)
                    {
                        ev.Player.RemoveHeldItem();
                    } else
                    {
                        ev.Player.DropHeldItem();
                    }

                    ev.CanSpawnImpactEffects = false;

                    if (UseRaycastPosition)
                    {
                        if (ev.RaycastHit.normal == new UnityEngine.Vector3(0, 1, 0))
                        {
                            ev.Player.Position = ev.RaycastHit.point + new UnityEngine.Vector3(0, 1, 0);
                        }
                    } 
                    else
                    {
                        ev.Player.Position = ev.Position;
                    }

                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (KillAfterMiss)
                        {
                            ev.Player.Kill(DeathReasonUser);
                        } 
                        else
                        {
                            ev.Player.Hurt(ev.Distance, "Body is badly mutilated.");
                        }
                    });
                }
                else
                {
                    if (DespawnAfterUse)
                    {
                        Log.Debug($"VVUP Custom Items: SCP-2818, Despawn After Use is true, removing SCP-2818 from {ev.Player.Nickname}'s inventory");
                        ev.Player.RemoveItem(ev.Item);
                    }

                    Log.Debug($"VVUP Custom Items: SCP-2818, Killing {ev.Player.Nickname}");
                    ev.Player.Kill(DeathReasonUser);
                }
            });
        }
    }
}