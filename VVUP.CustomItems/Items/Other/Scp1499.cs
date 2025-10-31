using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.SCP268)]
    public class Scp1499 : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.SCP268;
        public override uint Id { get; set; } = 29;
        public override string Name { get; set; } = "<color=#FF0000>SCP-1499</color>";
        public override string Description { get; set; } =
            "Is a gas mask that teleports you to another dimension when equipped";

        public override float Weight { get; set; } = 1.5f;

        [Description("Determines how long it lasts, 0 = infinite")]
        public float Duration { get; set; } = 15f;

        [Description("Determines if you can use SCP 1499 in 106's Pocket Dimension")]
        public bool UseInPocketDimension { get; set; } = false;

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
                    Location = SpawnLocationType.Inside079Secondary,
                },
            },
            StaticSpawnPoints = new List<StaticSpawnPoint>
            {
                new()
                {
                    Name = "Escape Hall Building",
                    Chance = 10,
                    Position = new Vector3(133, 289, 24),
                },
            },
        };
        
        [Description("Determines where to teleport the player, must be somewhere static (such as surface zone)")]
        public Vector3 TeleportLocation { get; set; } = new(38.464f, 314.112f, -32.689f);
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 127);
        public float GlowRange { get; set; } = 0.25f;
        
        private readonly Dictionary<PlayerAPI, Vector3> _playersUsingScp1499 = new();
        
        protected override void SubscribeEvents()
        {
            PlayerEvent.UsedItem += OnUsedItem;
            PlayerEvent.Destroying += OnDestroying;
            PlayerEvent.Died += OnDied;

            base.SubscribeEvents();
        }
        
        protected override void UnsubscribeEvents()
        {
            PlayerEvent.UsedItem -= OnUsedItem;
            PlayerEvent.Destroying -= OnDestroying;
            PlayerEvent.Died -= OnDied;

            base.UnsubscribeEvents();
        }

        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            if (_playersUsingScp1499.ContainsKey(ev.Player) && Check(ev.Item))
            {
                Log.Debug(
                    $"VVUP Custom Items, SCP1499: {ev.Player.Nickname} is currently using SCP1499, sending back to previous location");
                ev.IsAllowed = false;
                SendPlayerBack(ev.Player);
            }
        }

        protected override void OnWaitingForPlayers()
        {
            Log.Debug("VVUP Custom Items, SCP1499: Clearing any left over data from the previous round");
            _playersUsingScp1499.Clear();
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (_playersUsingScp1499.ContainsKey(ev.Player))
            {
                Log.Debug($"VVUP Custom Items, SCP1499: {ev.Player.Nickname} died, removing from list");
                _playersUsingScp1499.Remove(ev.Player);
            }
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            if (_playersUsingScp1499.ContainsKey(ev.Player))
            {
                Log.Debug($"VVUP Custom Items, SCP1499: Item destroyed, removing {ev.Player.Nickname} from list");
                _playersUsingScp1499.Remove(ev.Player);
            }
        }

        private void OnUsedItem(UsedItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ev.Player.Zone == ZoneType.Pocket && !UseInPocketDimension)
            {
                Log.Debug(
                    $"VVUP Custom Items, SCP1499: {ev.Player.Nickname} is in Pocket Dimension and usage in pocket dimension is disabled, stopping method");
                ev.Player.DisableEffect(EffectType.Invisible);
                return;
            }
            
            if (_playersUsingScp1499.ContainsKey(ev.Player))
            {
                Log.Debug($"VVUP Custom Items, SCP1499: {ev.Player.Nickname} is already on the list, saving their position ({ev.Player.Position})");
                _playersUsingScp1499[ev.Player] = ev.Player.Position;
            }
            else
            {
                Log.Debug($"VVUP Custom Items, SCP1499: Adding {ev.Player.Nickname} and their position ({ev.Player.Position}) to the list");
                _playersUsingScp1499.Add(ev.Player, ev.Player.Position);
            }
            
            Log.Debug($"VVUP Custom Items, SCP1499: Teleporting {ev.Player.Nickname} to {TeleportLocation}");
            ev.Player.Position = TeleportLocation;
            Log.Debug($"VVUP Custom Items, SCP1499: Removing invisibility from {ev.Player.Nickname}");
            ev.Player.DisableEffect(EffectType.Invisible);

            if (Duration > 0)
            {
                Log.Debug($"VVUP Custom Items, SCP1499: Waiting for {Duration} seconds");
                Timing.CallDelayed(Duration, () =>
                {
                    SendPlayerBack(ev.Player);
                });
            }
        }

        private void SendPlayerBack(PlayerAPI player)
        {
            if (!_playersUsingScp1499.ContainsKey(player))
                return;
            
            Log.Debug($"VVUP Custom Items, SCP1499: Teleporting {player.Nickname} position to {_playersUsingScp1499[player]}");
            player.Position = _playersUsingScp1499[player];
            
            if (Warhead.IsDetonated)
            {
                if (player.CurrentRoom.Zone != ZoneType.Surface)
                {
                    Log.Debug($"VVUP Custom Items, SCP1499: {player.Nickname}'s is in the facility when nuke went off, killing them");
                    player.Kill(DamageType.Warhead);
                }
                else if (player.Lift == Lift.Get(ElevatorType.GateA) || player.Lift == Lift.Get(ElevatorType.GateB))
                {
                    Log.Debug($"VVUP Custom Items, SCP1499: {player.Nickname}'s is in an elevator, killing them");
                    player.Kill(DamageType.Warhead);
                }
            }

            if (Map.IsLczDecontaminated && player.CurrentRoom.Zone == ZoneType.LightContainment)
            {
                Log.Debug($"VVUP Custom Items, SCP1499: {player.Nickname}'s is in light containment when decontamination started, killing them");
                player.Kill(DamageType.Decontamination);
            }
            
            _playersUsingScp1499.Remove(player);
        }
    }
}