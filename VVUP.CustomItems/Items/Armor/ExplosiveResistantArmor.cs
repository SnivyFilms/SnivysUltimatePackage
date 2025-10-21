using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using VVUP.CustomItems.API;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.Armor
{
    [CustomItem(ItemType.ArmorHeavy)]
    public class ExplosiveResistantArmor: CustomArmor, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.ArmorHeavy;
        public override uint Id { get; set; } = 25;
        public override string Name { get; set; } = "<color=#FF0000>Explosive Resistant Armor</color>";

        public override string Description { get; set; } =
            "When wearing this armor, you become resistant to explosives.";

        public override float Weight { get; set; } = 2f;
        [Description("This is a multiplicative, so 0.5 = half damage")]
        public float ExplosiveDamageReduction { get; set; } = 0.5f;
        private List<PlayerAPI> _playersWithArmorOn = new List<PlayerAPI>();
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHidChamber,
                },
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
            },
        };
        [Description("Must be between 1 and 2")]
        public override float StaminaUseMultiplier { get; set; } = 1.75f;
        [Description("Must be between 0 and 100")]
        public override int HelmetEfficacy { get; set; } = 85;
        [Description("Must be between 0 and 100")]
        public override int VestEfficacy { get; set; } = 85;
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 191);
        
        protected override void SubscribeEvents()
        {
            PlayerEvent.Hurting += OnHurting;
            PlayerEvent.Dying += OnDying;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.Hurting -= OnHurting;
            PlayerEvent.Dying -= OnDying;
            base.UnsubscribeEvents();
        }

        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (!_playersWithArmorOn.Contains(ev.Player))
                _playersWithArmorOn.Add(ev.Player);
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            _playersWithArmorOn.Remove(ev.Player);
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Explosion && _playersWithArmorOn.Contains(ev.Player))
            {
                Log.Debug("VVUP Custom Items: Explosion damage, reducing damage.");
                ev.DamageHandler.Damage *= ExplosiveDamageReduction;
            }
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (_playersWithArmorOn.Contains(ev.Player))
                _playersWithArmorOn.Remove(ev.Player);
        }
    }
}