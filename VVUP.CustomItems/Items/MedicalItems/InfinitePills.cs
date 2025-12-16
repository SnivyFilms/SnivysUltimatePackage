using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomItems.Items.MedicalItems
{
    [CustomItem(ItemType.Painkillers)]
    public class InfinitePills : CustomItem, ICustomItemGlow
    {
        public override uint Id { get; set; } = 34;
        public override string Name { get; set; } = "<color=#6600CC>Infinite Pills</color>";
        public override string Description { get; set; } = "This pill bottle seems endless\nUnfortunately it seems to be out of date and wont heal you";
        public override float Weight { get; set; } = 0.5f;
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.Painkillers;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new()
            {
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside939Cryo
                },
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Room = RoomType.HczTestRoom,
                    Offset = new Vector3(0.885f, 0.749f, -4.874f)
                },
            },
            LockerSpawnPoints = new()
            {
                new LockerSpawnPoint()
                {
                    Chance = 50,
                    Type = LockerType.Misc,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
            },
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItemCompleted += OnUsingItemCompleted;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItemCompleted -= OnUsingItemCompleted;
            base.UnsubscribeEvents();
        }

        private void OnUsingItemCompleted(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            ev.IsAllowed = false;
        }
    }
}