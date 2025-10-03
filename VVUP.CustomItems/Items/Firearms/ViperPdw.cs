using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem.Items.Firearms.Attachments;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunCrossvec)]
    public class ViperPdw : CustomWeapon
    {
        [YamlIgnore] 
        public override ItemType Type { get; set; } = ItemType.GunCrossvec;
        public override uint Id { get; set; } = 39;
        public override string Name { get; set; } = "<color=#FF0000>Viper</color>";
        public override string Description { get; set; } = "A compact PDW that does damage based on range to target";
        public override float Weight { get; set; } = 1.75f;

        [YamlIgnore]
        public override float Damage { get; set; } = 0;
        
        [Description("Due to damage being multiplicative, the damage can get really high really quick, so this divider keeps the damage (mostly) in check. Adjust this to your needs")]
        public int DamageDivider { get; set; } = 20;

        public override byte ClipSize { get; set; } = 10;
        public bool AllowAttachmentChanging { get; set; } = false;
        public string RestrictedAttachmentChangingMessage { get; set; } =
            "You're not allowed to swap attachments on the Viper";
        public bool UseHints { get; set; } = false;
        public float RestrictedAttachmentChangeMessageTimeDuration { get; set; } = 5f;
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
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
                    Location = SpawnLocationType.Inside096,
                },
                new ()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside079Armory,
                },
            }
        };
        
        public override AttachmentName[] Attachments { get; set; } = new[]
        {
            AttachmentName.None,
            AttachmentName.IronSights,
            AttachmentName.StandardBarrel,
            AttachmentName.RetractedStock,
            AttachmentName.StandardMagJHP,
        };
        private List<ushort> droppedVipers = new List<ushort>();

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Item.ChangingAttachments += OnChangingAttachments;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Item.ChangingAttachments -= OnChangingAttachments;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
            base.UnsubscribeEvents();
        }

        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (Check(ev.Pickup) && !droppedVipers.Contains(ev.Pickup.Serial))
            {
                Timing.CallDelayed(0.25f, () =>
                {
                    ev.Player.RemoveItem(ev.Pickup.Serial);
                    TryGive(ev.Player, Id, true);
                });
            }
        }
        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!droppedVipers.Contains(ev.Item.Serial))
                droppedVipers.Add(ev.Item.Serial);
        }
        private void OnRoundEnd(RoundEndedEventArgs ev)
        {
            droppedVipers.Clear();
        }

        protected override void OnWaitingForPlayers()
        {
            droppedVipers.Clear();
            base.OnWaitingForPlayers();
        }

        private void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem) && !AllowAttachmentChanging)
            {
                Log.Debug(
                    $"VVUP Custom Items: ViperPDW, {ev.Player.Nickname} tried changing attachments, but it's disallowed");
                ev.IsAllowed = false;
                if (UseHints)
                {
                    Log.Debug($"VVUP Custom Items: ViperPDW, showing Restricted Attachment Changing Message Hint to {ev.Player.Nickname} for {RestrictedAttachmentChangeMessageTimeDuration} seconds");
                    ev.Player.ShowHint(RestrictedAttachmentChangingMessage, RestrictedAttachmentChangeMessageTimeDuration);
                }
                else
                {
                    Log.Debug($"VVUP Custom Items: ViperPDW, showing Restricted Attachment Changing Message Broadcast to {ev.Player.Nickname} for {RestrictedAttachmentChangeMessageTimeDuration} seconds");
                    ev.Player.Broadcast((ushort)RestrictedAttachmentChangeMessageTimeDuration, RestrictedAttachmentChangingMessage);
                }
            }
        }

        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player == ev.Attacker)
                return;

            ev.IsAllowed = false;

            float distance = Vector3.Distance(ev.Player.Position, ev.Attacker.Position);
            float damageToApply = ev.Amount * distance / DamageDivider;

            ev.Attacker.ShowHitMarker(Mathf.Clamp(damageToApply / DamageDivider, 0.05f, 8f));
            ev.DamageHandler.Damage = damageToApply;
            ev.DamageHandler.ApplyDamage(ev.Player);
            ev.Player.Hurt(ev.DamageHandler.Damage);

            Log.Debug($"VVUP Custom Items: ViperPDW, {ev.Attacker.Nickname} attacked {ev.Player.Nickname} at distance {distance}, changing damage to {damageToApply}");
        }
    }
}