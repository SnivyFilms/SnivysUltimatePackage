using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;
using VVUP.Base;
using VVUP.Base.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomItems.Items.MedicalItems
{
    [CustomItem(ItemType.SCP500)]
    public class AntiScp096Pills : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override uint Id { get; set; } = 31;
        public override string Name { get; set; } = "<color=#6600CC>Amnesioflux</color>";
        public override string Description { get; set; } = "When consumed, it makes you no longer a target of SCP-096";
        public override float Weight { get; set; } = 1f;
        public List<ApplyEffects> Effects { get; set; } = new()
        {
            new ApplyEffects()
            {
                EffectType = EffectType.AmnesiaVision,
                Duration = 10,
                AddDurationIfActive = true,
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 100,
                    Offset = new Vector3(-3.25f, 1, -0.78f),
                    Room = RoomType.Hcz096,
                }
            },
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            base.SubscribeEvents();
        }
        
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            base.UnsubscribeEvents();
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Log.Debug($"VVUP Custom Items: Anti SCP 096 Pills, Removing {ev.Player} from 096's target list");
            IEnumerable<Player> scp096S = Player.Get(RoleTypeId.Scp096);

            Timing.CallDelayed(1f, () =>
            {
                foreach (Player scp in scp096S)
                {
                    if (scp.Role is Scp096Role scp096)
                    {
                        if (scp096.HasTarget(ev.Player))
                            scp096.RemoveTarget(ev.Player);
                    }
                }

                foreach (var effects in Effects)
                {
                    ev.Player.EnableEffect(effects.EffectType, effects.Intensity, effects.Duration,
                        effects.AddDurationIfActive);
                }
            });
        }
    }
}