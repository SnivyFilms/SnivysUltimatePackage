/*using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;
using Scp096EventArgs = Exiled.Events.EventArgs.Scp096;
using Scp096EventHandler = Exiled.Events.Handlers.Scp096;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.Coin)]
    public class CognitohazardCharm : CustomItem
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.Coin;

        public override uint Id { get; set; } = 52;
        public override string Name { get; set; } = "<color=#6600CC>Cognitohazard Charm</color>";
        public override string Description { get; set; } = "A charm that protects the wearer from cognitohazards.";
        public override float Weight { get; set; } = 0.25f;
        public bool ProtectsFrom096 { get; set; } = true;
        public string ProtectMessage096 { get; set; } = "Your <color=#6600CC>Cogititohazard Charm</color> protected you from SCP-096!";
        public float ProtectionDuration096 { get; set; } = 10f;
        public string RemovedItemMessage { get; set; } = "Your <color=#6600CC>Cognitohazard Charm</color> has been consumed.";
        public float MessageDuration { get; set; } = 5;
        public bool UseHints { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            LockerSpawnPoints = new()
            {
                new()
                {
                    Chance = 10,
                    Type = LockerType.Misc,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
            },
        };
        private Dictionary<Player, DateTime> _lastProtectionTime = new Dictionary<Player, DateTime>();
        protected override void SubscribeEvents()
        {
            Scp096EventHandler.AddingTarget += Adding096Target;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Scp096EventHandler.AddingTarget -= Adding096Target;
            base.UnsubscribeEvents();
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (_lastProtectionTime.TryGetValue(ev.Player, out DateTime lastUsed) &&
                (DateTime.Now - lastUsed).TotalSeconds < ProtectionDuration096)
            {
                ev.IsAllowed = false;
                ev.Player.RemoveItem(ev.Item);
                Log.Debug($"VVUP Custom Items: Removed Cognitohazard Charm from {ev.Player.Nickname} since the protection period is still active");
            }
        }

        private void Adding096Target(Scp096EventArgs.AddingTargetEventArgs ev)
        {
            if (!Check(ev.Target) || !ProtectsFrom096)
                return;
        
            if (_lastProtectionTime.TryGetValue(ev.Target, out DateTime lastUsed) && 
                (DateTime.Now - lastUsed).TotalSeconds < ProtectionDuration096)
            {
                ev.IsAllowed = false;
                return;
            }
            _lastProtectionTime[ev.Target] = DateTime.Now;
            ev.IsAllowed = false;
    
            if (UseHints)
                ev.Target.ShowHint(ProtectMessage096, MessageDuration);
            else
                ev.Target.Broadcast((ushort)MessageDuration, ProtectMessage096);
    
            Log.Debug($"VVUP Custom Items: Cognitohazard Charm protected {ev.Target.Nickname} from SCP-096 target list");
            
            Timing.CallDelayed(ProtectionDuration096, () =>
            {
                foreach (var item in ev.Target.Items.ToList().Where(Check))
                {
                    ev.Target.RemoveItem(item);
            
                    if (UseHints)
                        ev.Target.ShowHint(RemovedItemMessage, MessageDuration);
                    else
                        ev.Target.Broadcast((ushort)MessageDuration, RemovedItemMessage);
                
                    Log.Debug($"VVUP Custom Items: Removed Cognitohazard Charm from {ev.Target.Nickname} since the protection period expired");
                    return;
                }
            });
        }
    }
}*/