using System.Collections.Generic;
using System.ComponentModel;
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
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.MedicalItems
{
    [CustomItem(ItemType.Adrenaline)]
    public class KySyringe : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.Adrenaline;
        private bool KillAfterAnimation { get; set; } = true;
        private string KillReason { get; set; } = "Intentional Fatal Injection";
        public override uint Id { get; set; } = 26;
        public override string Name { get; set; } = "<color=#0000CC>LJ-429</color>";
        public override string Description { get; set; } = "When injected, the user has a quick death.";
        public override float Weight { get; set; } = 1.15f;
        [Description("Removes the Syringe on use, otherwise it just drops on the floor after the player dies (it's really funny ngl)")]
        public bool RemoveSyringeOnUse { get; set; } = true;

        public List<ApplyEffects> Effects { get; set; } = new()
        {
            new()
            {
                EffectType = EffectType.Corroding,
                Intensity = 1,
                Duration = 300,
                AddDurationIfActive = false
            }
        };
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = SpawnLocationType.Inside939Cryo,
                },
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Offset = new Vector3(-3.25f, 1, -0.78f),
                    Room = RoomType.Hcz096,
                }
            },
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        
        protected override void SubscribeEvents()
        {
            if (KillAfterAnimation)
                PlayerEvents.UsingItemCompleted += OnUsingLJAnimation;
            else
                PlayerEvents.UsingItem += OnUsingLJ;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            if (KillAfterAnimation)
                PlayerEvents.UsingItemCompleted -= OnUsingLJAnimation;
            else
                PlayerEvents.UsingItem -= OnUsingLJ;

            base.UnsubscribeEvents();
        }
        private void OnUsingLJ(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Log.Debug($"VVUP Custom Items: KY Syringe, Killing {ev.Player.Nickname}");
            if (RemoveSyringeOnUse)
                ev.Player.RemoveItem(ev.Item);
            KillPlayer(ev.Player);
        }

        private void OnUsingLJAnimation(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Log.Debug($"VVUP Custom Items: KY Syringe, Killing {ev.Player.Nickname}");
            if (RemoveSyringeOnUse)
                ev.Player.RemoveItem(ev.Item);
            KillPlayer(ev.Player);
        }

        private void KillPlayer(Player player)
        {
            player.Kill(KillReason);
            player.Health = 1;
            foreach (var effects in Effects)
            {
                player.EnableEffect(effects.EffectType, effects.Intensity, effects.Duration, effects.AddDurationIfActive);
            }
        }
    }
}