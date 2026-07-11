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

namespace VVUP.CustomItems.Items.MedicalItems
{
    [CustomItem(ItemType.Adrenaline)]
    public class ExperimentalStim : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type => ItemType.Adrenaline;
        public override uint Id { get; set; } = 60;
        public override string Name { get; set; } = "<color=#6600CC>Experimental Stim</color>";

        public override string Description { get; set; } =
            "When used, will provide a random effect, either positive or negative";

        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Room = RoomType.HczTestRoom,
                    Offset = new Vector3(0.885f, 0.749f, -4.874f)
                },
            },
        };

        public List<ApplyEffects> Effects { get; set; } = new()
        {
            new ApplyEffects()
            {
                EffectType = EffectType.MovementBoost,
                Intensity = 50,
                Duration = 10,
            },
            new ApplyEffects()
            {
                EffectType = EffectType.Invigorated,
                Intensity = 1,
                Duration = 30,
            },
            new ApplyEffects()
            {
                EffectType = EffectType.Poisoned,
                Intensity = 50,
                Duration = 60,
            },
            new ApplyEffects()
            {
                EffectType = EffectType.Invisible,
                Intensity = 1,
                Duration = 15,
            },
            new ApplyEffects()
            {
                EffectType = EffectType.DamageReduction,
                Intensity = 50,
                Duration = 30,
            },
            new ApplyEffects()
            {
                EffectType = EffectType.Decontaminating,
                Intensity = 1,
                Duration = 60,
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
            if (ev.Player == null)
                return;
            ev.IsAllowed = false;
            ev.Player.RemoveItem(ev.Item);
            if (Effects == null || Effects.Count == 0)
            {
                Log.Warn($"VVUP Custom Items, Experimental Sim: {ev.Player.Nickname} tried using the item, but the effect list is empty. Just taking the item and doing nothing.");
                return;
            }
            var randomEffect = Effects[GetRandomNumber.GetRandomInt(0, Effects.Count)];
            ev.Player.EnableEffect(randomEffect.EffectType, randomEffect.Intensity, randomEffect.Duration);
            Log.Debug($"VVUP Custom Items, Experimental Sim: {ev.Player.Nickname} used the item and received {randomEffect.EffectType} with intensity {randomEffect.Intensity} for {randomEffect.Duration} seconds.");
        }
    }
}