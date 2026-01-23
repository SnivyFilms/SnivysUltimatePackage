using CustomPlayerEffects;
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
using VVUP.Base.API;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunE11SR)]
    public class Sniper : CustomWeapon, ICustomItemGlow
    {
        public override string Name { get; set; } = "EL-95 Bolt Action Rifle";
        public override string Description { get; set; } = "Bolt action sniper rifle.";
        public override ItemType Type => ItemType.GunE11SR;
        public override float Weight { get; set; } = 1.5f;
        public override AttachmentName[] Attachments => [AttachmentName.LowcapMagAP, AttachmentName.ExtendedStock, AttachmentName.RifleBody, AttachmentName.Laser, AttachmentName.Foregrip, AttachmentName.ScopeSight, AttachmentName.SoundSuppressor];
        public override byte ClipSize { get; set; } = 1;
        public override float Damage { get; set; } = 150f;
        public override uint Id { get; set; } = 58;
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

        [Description("If true Slowness will be given when aiming. Default: true")]
        public bool ApplySlownessOnAim { get; set; } = true;

        [Description("Intensity of the slowness effect applied when aiming down sights. Default: 5")]
        public byte SlownessIntensity { get; set; } = 5;

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

            ev.IsAllowed = false;
            ev.Player.ShowHint($"You cannot modify the attachments of the {Name}");
            Log.Debug($"{ev.Player.Nickname} - {ev.Player.Id} Tried to change attachments on {Name}");
            base.OnChangingAttachment(ev);
        }

        public void OnAim(AimingDownSightEventArgs ev)
        {
            if (!ApplySlownessOnAim || !Check(ev.Item))
                return;

            if (ev.AdsIn)
            {
                ev.Player.EnableEffect<Slowness>(SlownessIntensity, float.MaxValue, false);
            }
            else
                ev.Player.DisableEffect<Slowness>();
        }
    }
}
