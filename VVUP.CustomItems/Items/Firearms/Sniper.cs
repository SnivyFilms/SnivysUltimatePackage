using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using System.ComponentModel;
using UnityEngine;
using VVUP.Base;
using VVUP.Base.API;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunE11SR)]
    public class Sniper : CustomWeapon, ICustomItemGlow
    {
        public override ItemType Type { get; set; } = ItemType.GunE11SR;
        public override uint Id { get; set; } = 58;
        public override string Name { get; set; } = "<color=#009F64>EL-95 Bolt Action Rifle</color>";
        public override string Description { get; set; } = "Bolt action sniper rifle.";
        public override float Weight { get; set; } = 1.5f;
        public override AttachmentName[] Attachments => [AttachmentName.LowcapMagAP, AttachmentName.ExtendedStock, AttachmentName.RifleBody, AttachmentName.Laser, AttachmentName.Foregrip, AttachmentName.ScopeSight, AttachmentName.SoundSuppressor];
        public override byte ClipSize { get; set; } = 1;
        public override float Damage { get; set; } = 150f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints =
            [
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new ()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside079Armory,
                },
                new ()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHczArmory,
                },
            ],
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.AimingDownSight += OnAim;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.AimingDownSight -= OnAim;
            base.UnsubscribeEvents();
        }

        [Description("If true changing the attachments will be possible. Default: false")]
        public bool AllowChangingAttachments { get; set; } = false;
        public string RestrictedAttachmentChangingMessage { get; set; } =
            "You're not allowed to swap attachments on the Explosive Round Pistol";
        public bool UseHints { get; set; } = false;
        public float RestrictedAttachmentChangeMessageTimeDuration { get; set; } = 5f;

        [Description("If its not empty, effects will be applied when aiming down sights.")]
        public List<ApplyEffects> ApplyEffectsOnAim { get; set; } = new()
        {
            new ApplyEffects()
            {
                EffectType = EffectType.Slowness,
                Intensity = 255,
                Duration = 0,
            },
        };

        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 150, 100, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

        protected override void OnChangingAttachment(ChangingAttachmentsEventArgs ev)
        {
            if (!Check(ev.Item) || AllowChangingAttachments)
                return;
            if (!string.IsNullOrWhiteSpace(RestrictedAttachmentChangingMessage)) 
                if (UseHints)
                {
                    Log.Debug(
                        $"VVUP Custom Items: Sniper, showing Restricted Attachment Changing Message Hint to {ev.Player.Nickname} for {RestrictedAttachmentChangeMessageTimeDuration} seconds");
                    ev.Player.ShowHint(RestrictedAttachmentChangingMessage,
                        RestrictedAttachmentChangeMessageTimeDuration);
                }
                else
                {
                    Log.Debug(
                        $"VVUP Custom Items: Sniper, showing Restricted Attachment Changing Message Broadcast to {ev.Player.Nickname} for {RestrictedAttachmentChangeMessageTimeDuration} seconds");
                    ev.Player.Broadcast((ushort)RestrictedAttachmentChangeMessageTimeDuration,
                        RestrictedAttachmentChangingMessage);
                }
            ev.IsAllowed = false;
            base.OnChangingAttachment(ev);
        }

        public void OnAim(AimingDownSightEventArgs ev)
        {
            if (ApplyEffectsOnAim.IsEmpty() || !Check(ev.Item))
                return;

            if (ev.AdsIn)
            {
                foreach(var effect in ApplyEffectsOnAim)
                {
                    ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
                    Log.Debug($"VVUP Custom Items: Sniper, applied effect {effect.EffectType} with intensity {effect.Intensity} for {effect.Duration} seconds to {ev.Player.Nickname}");
                }
            }
            else
            {
                foreach(var effect in ApplyEffectsOnAim)
                {
                    ev.Player.DisableEffect(effect.EffectType);
                    Log.Debug($"VVUP Custom Items: Sniper, removed effect {effect.EffectType} from {ev.Player.Nickname}");
                }
            }
        }
    }
}
