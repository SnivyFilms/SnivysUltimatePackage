using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using YamlDotNet.Serialization;

namespace VVUP.CustomItems.Items.Firearms
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
                new ()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside079Armory,
                },
            }
        };

        public override AttachmentName[] Attachments { get; set; } = new[]
        {
            AttachmentName.CylinderMag5,
            AttachmentName.ExtendedBarrel,
            AttachmentName.IronSights,
        };

        public override float Damage { get; set; } = 0;
        public override byte ClipSize { get; set; } = 2;
        public float FuseTime { get; set; } = 2.5f;
        public float ScpGrenadeDamageMultiplier { get; set; } = .5f;
        public bool AllowAttachmentChanging { get; set; } = false;
        public string RestrictedAttachmentChangingMessage { get; set; } =
            "You're not allowed to swap attachments on the Explosive Round Revolver";
        public bool UseHints { get; set; } = false;
        public float RestrictedAttachmentChangeMessageTimeDuration { get; set; } = 5f;
        private List<ushort> droppedRevolvers = new List<ushort>();

        protected override void SubscribeEvents()
        {
            //Player.ReloadingWeapon += OnReloading;
            Exiled.Events.Handlers.Item.ChangingAttachments += OnChangingAttachments;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            //Player.ReloadingWeapon -= OnReloading;
            Exiled.Events.Handlers.Item.ChangingAttachments -= OnChangingAttachments;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
            base.UnsubscribeEvents();
        }

        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (Check(ev.Pickup) && !droppedRevolvers.Contains(ev.Pickup.Serial))
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
            if (!droppedRevolvers.Contains(ev.Item.Serial))
                droppedRevolvers.Add(ev.Item.Serial);
        }
        private void OnRoundEnd(RoundEndedEventArgs ev)
        {
            droppedRevolvers.Clear();
        }

        protected override void OnWaitingForPlayers()
        {
            droppedRevolvers.Clear();
            base.OnWaitingForPlayers();
        }
        
        private void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem) && !AllowAttachmentChanging)
            {
                Log.Debug($"VVUP Custom Items: Explosive Round Revolver, {ev.Player.Nickname} tried changing attachments, but it's disallowed");
                ev.IsAllowed = false;
                if (UseHints)
                {
                    Log.Debug($"VVUP Custom Items: Explosive Round Revolver, showing Restricted Attachment Changing Message Hint to {ev.Player.Nickname} for {RestrictedAttachmentChangeMessageTimeDuration} seconds");
                    ev.Player.ShowHint(RestrictedAttachmentChangingMessage, RestrictedAttachmentChangeMessageTimeDuration);
                }
                else
                {
                    Log.Debug($"VVUP Custom Items: Explosive Round Revolver, showing Restricted Attachment Changing Message Broadcast to {ev.Player.Nickname} for {RestrictedAttachmentChangeMessageTimeDuration} seconds");
                    ev.Player.Broadcast((ushort)RestrictedAttachmentChangeMessageTimeDuration, RestrictedAttachmentChangingMessage);
                }
            }
        }
       /* private void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Timing.CallDelayed(3f, () =>
            {
                Log.Debug($"VVUP Custom Items: Explosive Round Revolver, {ev.Player.Nickname} has started reloading, setting correct ammo");
                ev.Firearm.MagazineAmmo = ClipSize;
            });
        }*/
       protected override void OnShot(ShotEventArgs ev)
       {
           if (!Check(ev.Player.CurrentItem))
               return;
           Log.Debug($"VVUP Custom Items: Explosive Round Revolver, spawning grenade at {ev.Position}");
           ev.CanHurt = false;
            
           try
           {
               ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
               grenade.FuseTime = FuseTime;
               grenade.ScpDamageMultiplier = ScpGrenadeDamageMultiplier;
               grenade.SpawnActive(ev.Position, owner: ev.Player);
           }
           catch (System.Exception ex)
           {
               Log.Error($"VVUP Custom Items: Error spawning explosive round: {ex.Message}");
           }
       }
    }
}