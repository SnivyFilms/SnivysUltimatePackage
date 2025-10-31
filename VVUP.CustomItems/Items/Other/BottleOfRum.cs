using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using VVUP.Base;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.SCP207)]
    public class BottleOfRum : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.SCP207;

        public override uint Id { get; set; } = 53;
        public override string Name { get; set; } = "<color=#6600CC>Bottle of Rum</color>";
        public override string Description { get; set; } = "seashanty2.mp3";
        public override float Weight { get; set; } = 1;
        
        public List<ApplyEffects> Effects { get; set; } = new()
        {
            new ApplyEffects()
            {
                EffectType = EffectType.Slowness,
                Intensity = 255,
                Duration = 40,
            },
            new ApplyEffects()
            {
                EffectType = EffectType.Blurred,
                Intensity = 1,
                Duration = 40,
            },
            new ApplyEffects()
            {
                EffectType = EffectType.Blinded,
                Intensity = 50,
                Duration = 40,
            },
            new ApplyEffects()
            {
                EffectType = EffectType.Invigorated,
                Intensity = 1,
                Duration = 40,
            },
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
            LockerSpawnPoints = new()
            {
                new()
                {
                    Chance = 35,
                    Type = LockerType.Misc,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 35,
                    Type = LockerType.Misc,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 35,
                    Type = LockerType.Misc,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 35,
                    Type = LockerType.Misc,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 35,
                    Type = LockerType.Misc,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
            },
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;

        protected override void SubscribeEvents()
        {
            PlayerEvent.UsingItemCompleted += OnUsingItemCompleted;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.UsingItemCompleted -= OnUsingItemCompleted;
            base.UnsubscribeEvents();
        }

        private void OnUsingItemCompleted(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
            ev.IsAllowed = false;
            Log.Debug($"VVUP Custom Items: Rum, removing item from {ev.Player.Nickname}");
            ev.Player.RemoveItem(ev.Item);
            foreach (var effect in Effects)
            {
                ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
                Log.Debug($"VVUP Custom Items: Rum, applied effect {effect.EffectType} with intensity {effect.Intensity} for {effect.Duration} seconds to {ev.Player.Nickname}");
            }
        }
    }
}