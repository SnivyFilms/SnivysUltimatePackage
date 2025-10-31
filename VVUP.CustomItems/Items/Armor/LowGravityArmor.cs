using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using PlayerLab = LabApi.Features.Wrappers.Player;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.Armor
{
    [CustomItem(ItemType.ArmorLight)]
    public class LowGravityArmor : CustomArmor, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.ArmorLight;
        public override uint Id { get; set; } = 38;
        public override string Name { get; set; } = "<color=#6600CC>Nebula Carapace</color>";

        public override string Description { get; set; } =
            "This armor is so light-weight that it feels like you're far lighter";

        public override float Weight { get; set; } = 0.75f;

        public Vector3 GravityChanges { get; set; } = new Vector3()
        {
            x = 0,
            y = -9.8f,
            z = 0
        };
        
        [YamlIgnore]
        private Dictionary<PlayerAPI, Vector3> _playersWithArmorOn = new Dictionary<PlayerAPI, Vector3>();

        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGr18Glass,
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
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside173Bottom,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside939Cryo,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideLczCafe,
                },
            },
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        
        protected override void SubscribeEvents()
        {
            PlayerEvent.Dying += OnDying;
            PlayerEvent.ItemRemoved += OnItemRemoved;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.Dying -= OnDying;
            PlayerEvent.ItemRemoved -= OnItemRemoved;
            base.UnsubscribeEvents();
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (!_playersWithArmorOn.ContainsKey(player))
            {
                Log.Debug($"VVUP Custom Items: Low Gravity Armor, {player.Nickname} has put on Low Gravity Armor, gravity will be set to {GravityChanges}.");
                Vector3 previousGravity = PlayerLab.Get(player.NetworkIdentity)!.Gravity;
                _playersWithArmorOn[player] = previousGravity;
                PlayerLab.Get(player.NetworkIdentity)!.Gravity = GravityChanges;
            }
            base.OnAcquired(player, item, displayMessage);
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (_playersWithArmorOn.ContainsKey(ev.Player) && Check(ev.Item))
            {
                PlayerLab.Get(ev.Player.NetworkIdentity)!.Gravity = _playersWithArmorOn[ev.Player];
                _playersWithArmorOn.Remove(ev.Player);
                Log.Debug($"VVUP Custom Items: Low Gravity Armor, {ev.Player.Nickname} has taken off Low Gravity Armor, setting gravity back to {PlayerLab.Get(ev.Player.NetworkIdentity)?.Gravity.ToString()}.");
            }
            base.OnDroppingItem(ev);
        }
        
        private void OnItemRemoved(ItemRemovedEventArgs ev)
        {
            if (_playersWithArmorOn.ContainsKey(ev.Player) && Check(ev.Item))
            {
                PlayerLab.Get(ev.Player.NetworkIdentity)!.Gravity = _playersWithArmorOn[ev.Player];
                _playersWithArmorOn.Remove(ev.Player);
                Log.Debug($"VVUP Custom Items: Low Gravity Armor, {ev.Player.Nickname} has taken off Low Gravity Armor, setting gravity back to {PlayerLab.Get(ev.Player.NetworkIdentity)?.Gravity.ToString()}.");
            }
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (_playersWithArmorOn.ContainsKey(ev.Player))
            {
                PlayerLab.Get(ev.Player.NetworkIdentity)!.Gravity = _playersWithArmorOn[ev.Player];
                _playersWithArmorOn.Remove(ev.Player);
                Log.Debug($"VVUP Custom Items: Low Gravity Armor, {ev.Player.Nickname} has taken off Low Gravity Armor, setting gravity back to {PlayerLab.Get(ev.Player.NetworkIdentity)?.Gravity.ToString()}.");
            }
        }

        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            if (_playersWithArmorOn.ContainsKey(ev.Player) && !ev.Player.Items.Any(item => Check(item)))
            {
                PlayerLab.Get(ev.Player.NetworkIdentity)!.Gravity = _playersWithArmorOn[ev.Player];
                _playersWithArmorOn.Remove(ev.Player);
                Log.Debug($"VVUP Custom Items: Low Gravity Armor, {ev.Player.Nickname} has lost Low Gravity Armor through inventory change, setting gravity back to {PlayerLab.Get(ev.Player.NetworkIdentity)?.Gravity.ToString()}.");
            }
            base.OnChanging(ev); 
        }
    }
}