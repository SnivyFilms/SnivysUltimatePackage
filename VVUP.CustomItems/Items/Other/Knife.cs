using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using Scp1509Event = Exiled.Events.Handlers.Scp1509;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.SCP1509)]
    public class Knife : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.SCP1509;
        public override uint Id { get; set; } = 57;
        public override string Name { get; set; } = "<color=#00FF00>Knife</color>";
        public override string Description { get; set; } = "A sharp blade used for close combat.";
        public override float Weight { get; set; } = 1;
        [Description("Cooldown time in seconds between swings.")]
        public float SwingCooldown { get; set; } = 1.5f;
        [Description("Use {time} to fetch the remaining cooldown time.")]
        public string CooldownMessage { get; set; } = "Knife is on cooldown for {time} more seconds.";
        public float MessageDuration { get; set; } = 5f;
        public bool UseHints { get; set; } = true;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new()
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside049Armory
                },
            },
            LockerSpawnPoints = new()
            {
                new()
                {
                    Chance = 10,
                    Type = LockerType.LargeGun
                }
            }
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 255, 0, 127);
        public float GlowRange { get; set; } = 0.2f;
        public float GlowIntensity { get; set; } = 0.25f;

        private readonly Dictionary<Player, float> _lastSwingTime = new();

        protected override void SubscribeEvents()
        {
            Scp1509Event.Resurrecting += On1509Resurrecting;
            Scp1509Event.TriggeringAttack += On1509Attack;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Scp1509Event.Resurrecting -= On1509Resurrecting;
            Scp1509Event.TriggeringAttack -= On1509Attack;
            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            _lastSwingTime.Clear();
            Log.Debug("VVUP Custom Items, Knife: Cleared last swing times due to waiting for players.");
            base.OnWaitingForPlayers();
        }

        private void On1509Resurrecting(Exiled.Events.EventArgs.Scp1509.ResurrectingEventArgs ev)
        {
            if (!Check(ev.Player))
                return;
            Log.Debug($"VVUP Custom Items, Knife: Prevented resurrection of {ev.Target} by {ev.Player}");
            ev.IsAllowed = false;
        }
        private void On1509Attack(Exiled.Events.EventArgs.Scp1509.TriggeringAttackEventArgs ev)
        {
            if (!Check(ev.Player))
                return;
            
            float currentTime = Time.time;
    
            if (_lastSwingTime.TryGetValue(ev.Player, out float lastTime))
            {
                if (currentTime - lastTime < SwingCooldown)
                {
                    var cooldownTimeRemaining = SwingCooldown - currentTime - lastTime;
                    ev.IsAllowed = false;
                    if (!string.IsNullOrWhiteSpace(CooldownMessage))
                        if (UseHints)
                            ev.Player.ShowHint(CooldownMessage.Replace("{percent}", cooldownTimeRemaining.ToString()), MessageDuration);
                        else
                            ev.Player.Broadcast((ushort)MessageDuration, CooldownMessage.Replace("{percent}", cooldownTimeRemaining.ToString()));
                    
                    Log.Debug($"VVUP Custom Items, Knife: Attack by {ev.Player} blocked due to cooldown");
                    return;
                }
            }
    
            _lastSwingTime[ev.Player] = currentTime;
            Log.Debug($"VVUP Custom Items, Knife: Allowed swing by {ev.Player}");
            ev.IsAllowed = true;
        }
    }
}