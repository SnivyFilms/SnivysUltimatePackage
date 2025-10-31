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
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunCOM18)]
    public class Pathfinder : CustomWeapon, ICustomItemGlow
    {
        [YamlIgnore] public override ItemType Type { get; set; } = ItemType.GunCOM18;
        public override uint Id { get; set; } = 40;
        public override string Name { get; set; } = "<color=#0096FF>Pathfinder</color>";

        public override string Description { get; set; } =
            "When it hits a hostile, they get marked for a set amount of time, causing increased damage";

        public override float Weight { get; set; } = 1.5f;
        public override SpawnProperties SpawnProperties { get; set; }
        public override float Damage { get; set; } = 0;
        public override byte ClipSize { get; set; } = 1;

        [Description("Handles how long a target should be marked for")]
        public float MarkedDuration { get; set; } = 10f;
        [Description("Handles the damage multipler the tracked target(s) should take (1.10 = 10% more, 1.50 = 50% more, etc")]
        public float DamageMultiplier { get; set; } = 1.10f;
        [Description("If true on a friendly fire on server. Can the Pathfinder mark teammates. If set to true on a friendly fire off server, does nothing.")]
        public bool MarkTeammates { get; set; } = false;
        
        private List<PlayerAPI> _activeMarkedPlayers = new();
        private List<PlayerAPI> _playersReloading = new();
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 150, 255, 127);
        public float GlowRange { get; set; } = 0.25f;
        
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloading;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloading;
            base.UnsubscribeEvents();
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            if (ev.Target == null)
                return;
            if (ev.Player.Role.Side == Side.Mtf && (ev.Target.Role.Side != Side.Mtf || 
                (ev.Target.Role.Side == Side.Mtf && MarkTeammates)))
            {
                if (_activeMarkedPlayers.Contains(ev.Target))
                    return;
                _activeMarkedPlayers.Add(ev.Target);
            }
            else if (ev.Player.Role.Side == Side.ChaosInsurgency && (ev.Target.Role.Side != Side.ChaosInsurgency ||
                     (ev.Target.Role.Side == Side.ChaosInsurgency && MarkTeammates)))
            {
                if (_activeMarkedPlayers.Contains(ev.Target))
                    return;
                _activeMarkedPlayers.Add(ev.Target);
            }
            else if (ev.Player.Role.Side is Side.Tutorial or Side.Scp && (ev.Target.Role.Side is not Side.Tutorial or Side.Scp ||
                     (ev.Target.Role.Side is Side.Tutorial or Side.Scp && MarkTeammates)))
            {
                if (_activeMarkedPlayers.Contains(ev.Target))
                    return;
                _activeMarkedPlayers.Add(ev.Target);
            }
            Timing.CallDelayed(MarkedDuration, () =>
            {
                _activeMarkedPlayers.Remove(ev.Target);
            });
        }

        private new void OnHurting(HurtingEventArgs ev)
        {
            if (!_activeMarkedPlayers.Contains(ev.Player))
                return;
            ev.Amount *= DamageMultiplier;
        }

        private void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Timing.CallDelayed(2f, () =>
            {
                Log.Debug($"VVUP Custom Items: Pathfinder, {ev.Player.Nickname} has started reloading, setting correct ammo");
                ev.Firearm.MagazineAmmo = ClipSize;
            });
        }
    }
}