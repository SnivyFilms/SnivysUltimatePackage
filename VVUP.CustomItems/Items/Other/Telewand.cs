using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using YamlDotNet.Serialization;
using UnityEngine;
using MEC;
using System.ComponentModel;
using VVUP.CustomItems.API;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.Jailbird)]
    public class Telewand : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.Jailbird;
        public override uint Id { get; set; } = 50;
        public override string Name { get; set; } = "<color=#0096FF>TeleWand</color>";
        public override string Description { get; set; } = "<b>LMB</b> to save position, <b>RMB</b> to teleport";
        public override float Weight { get; set; } = 1;

        [Description("Time before the item can be used again after teleporting")]
        public float UseCooldown { get; set; } = 15f;

        [Description("Delay after using the item before the player is teleported")]
        public float TeleportCooldown { get; set; } = 5f;

        [Description("Maximum number of times the item can be used before breaking")]
        public int MaxUses { get; set; } = 3;

        [Description("Maximum distance for teleportation (0 for unlimited)")]
        public float MaxTeleportRange { get; set; } = 0;

        [Description("List of rooms where position saving is not allowed")]
        public List<RoomType> RestrictedSaveRooms { get; set; } = new List<RoomType>
        {
            RoomType.EzCollapsedTunnel,
            RoomType.Pocket
        };

        [Description("List of rooms where teleporting from is not allowed")]
        public List<RoomType> RestrictedTeleportFromRooms { get; set; } = new List<RoomType>
        {
            RoomType.EzCollapsedTunnel,
            RoomType.Pocket
        };

        [Description("Spawn Location:")]
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 150, 255, 191);

        [Description("Translations:")]
        public string PosSaved { get; set; } = "<color=#808080>Position Saved</color>";
        public string NoPosSaved { get; set; } = "<color=#FF5554>No position saved!</color>";
        public string TpCooldown { get; set; } = "<color=#0096FF>TeleWand</color>: Wait {remaining}s before next use";
        public string TpInProgress { get; set; } = "Don't move for <color=yellow>{remaining}s</color> to teleport!";
        public string TpCanceled { get; set; } = "<color=#FF5554>Teleportation Canceled!</color>";
        public string TpSucceeded { get; set; } = "<color=#54FF54>Teleportation Succeeded!</color>";
        public string AfterWarhead { get; set; } = "<color=#FF5554>Warhead detonated! You can only teleport to the surface.</color>";
        public string AfterDecontamination { get; set; } = "<color=#FF5554>Cannot teleport to Light Containment - zone is decontaminated!</color>";
        public string ToPocketDimension { get; set; } = "<color=#FF5554>Cannot teleport to Pocket Dimension!</color>";
        public string TelewandBroken { get; set; } = "<color=#0096FF>TeleWand</color> has broken!";
        public string RestrictedSaveRoom { get; set; } = "<color=#FF5554>Cannot save position in this room!</color>";
        public string RestrictedTeleportFromRoom { get; set; } = "<color=#FF5554>Cannot teleport from this room!</color>";
        public string OutOfRange { get; set; } = "<color=#FF5554>Destination is out of range!</color>";


        private readonly Dictionary<Player, Vector3> savedPositions = new();
        private readonly Dictionary<ushort, CoroutineHandle> activeCountdowns = new();
        private readonly Dictionary<ushort, float> lastUseTimes = new();
        private readonly Dictionary<ushort, int> useCounts = new();
        private readonly Dictionary<Player, CoroutineHandle> teleportHints = new();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Item.ChargingJailbird += OnCharging;
            Exiled.Events.Handlers.Item.Swinging += OnSwinging;
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Item.ChargingJailbird -= OnCharging;
            Exiled.Events.Handlers.Item.Swinging -= OnSwinging;
            base.UnsubscribeEvents();
        }

        private void OnSwinging(SwingingEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;

                if (RestrictedSaveRooms.Contains(ev.Player.CurrentRoom.Type))
                {
                    ev.Player.ShowHint(RestrictedSaveRoom, 2f);
                    return;
                }

                savedPositions[ev.Player] = ev.Player.Position;
                ev.Player.ShowHint(PosSaved, 2f);
            }
        }

        private void OnCharging(ChargingJailbirdEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem))
            {
                ev.IsAllowed = false;

                if (RestrictedTeleportFromRooms.Contains(ev.Player.CurrentRoom.Type))
                {
                    ev.Player.ShowHint(RestrictedTeleportFromRoom, 2f);
                    return;
                }

                ushort serial = ev.Item.Serial;

                if (lastUseTimes.TryGetValue(serial, out float lastTime))
                {
                    float now = Time.realtimeSinceStartup;
                    if (now - lastTime < UseCooldown)
                    {
                        float left = UseCooldown - (now - lastTime);
                        string hint = TpCooldown.Replace("{remaining}", left.ToString("F1"));

                        ev.Player.ShowHint(hint, 2f);
                        return;
                    }
                }

                if (activeCountdowns.ContainsKey(serial))
                    return;

                if (MaxTeleportRange > 0 && savedPositions.TryGetValue(ev.Player, out Vector3 savedPos))
                {
                    float distance = Vector3.Distance(ev.Player.Position, savedPos);
                    if (distance > MaxTeleportRange)
                    {
                        ev.Player.ShowHint(OutOfRange, 2f);
                        return;
                    }
                }

                CoroutineHandle handle = Timing.RunCoroutine(TeleportCountdown(ev.Player, serial));
                activeCountdowns[serial] = handle;

                ClearTeleportHint(ev.Player);

                CoroutineHandle hintHandle = Timing.RunCoroutine(ShowTeleportHint(ev.Player, TeleportCooldown));
                teleportHints[ev.Player] = hintHandle;
            }
        }

        private IEnumerator<float> ShowTeleportHint(Player player, float duration)
        {
            float end = Time.realtimeSinceStartup + duration;

            while (Time.realtimeSinceStartup < end)
            {
                float left = end - Time.realtimeSinceStartup;
                string hint = TpInProgress.Replace("{remaining}", left.ToString("F1"));

                player.ShowHint(hint, 0.2f);
                yield return 0.1f;
            }
        }

        private IEnumerator<float> TeleportCountdown(Player player, ushort serial)
        {
            Vector3 startPos = player.Position;
            float startTime = Time.realtimeSinceStartup;
            float duration = TeleportCooldown;

            while (Time.realtimeSinceStartup - startTime < duration)
            {
                if (Vector3.Distance(player.Position, startPos) > 0.05f)
                {
                    player.ShowHint(TpCanceled, 2f);

                    activeCountdowns.Remove(serial);
                    ClearTeleportHint(player);

                    yield break;
                }

                yield return 0.1f;
            }

            if (savedPositions.TryGetValue(player, out Vector3 dest))
            {
                var destRoom = Room.Get(dest);

                if (destRoom != null)
                {
                    if (Warhead.IsDetonated && destRoom.Zone != ZoneType.Surface)
                    {
                        player.ShowHint(AfterWarhead, 3f);

                        activeCountdowns.Remove(serial);
                        ClearTeleportHint(player);

                        yield break;
                    }

                    if (Map.IsLczDecontaminated && destRoom.Zone == ZoneType.LightContainment)
                    {
                        player.ShowHint(AfterDecontamination, 3f);

                        activeCountdowns.Remove(serial);
                        ClearTeleportHint(player);

                        yield break;
                    }

                    if (destRoom.Zone == ZoneType.Pocket)
                    {
                        player.ShowHint(ToPocketDimension, 3f);

                        activeCountdowns.Remove(serial);
                        ClearTeleportHint(player);

                        yield break;
                    }
                }

                player.Position = dest;
                player.ShowHint(TpSucceeded, 2f);

                useCounts[serial] = useCounts.TryGetValue(serial, out int count) ? count + 1 : 1;
                lastUseTimes[serial] = Time.realtimeSinceStartup;

                if (useCounts[serial] >= MaxUses)
                {
                    var item = player.Items.FirstOrDefault(i => i.Serial == serial);

                    if (item != null)
                    {
                        player.RemoveItem(item);
                        player.ShowHint(TelewandBroken, 3f);
                    }

                    activeCountdowns.Remove(serial);
                }
            }
            else
            {
                player.ShowHint(NoPosSaved, 2f);
            }

            ClearTeleportHint(player);
            activeCountdowns.Remove(serial);
        }

        private void ClearTeleportHint(Player player)
        {
            if (teleportHints.TryGetValue(player, out var handle))
            {
                Timing.KillCoroutines(handle);
                teleportHints.Remove(player);
            }
        }
    }
}
