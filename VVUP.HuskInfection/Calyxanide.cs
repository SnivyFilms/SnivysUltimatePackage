using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using VVUP.CustomItems.API;
using YamlDotNet.Serialization;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.HuskInfection
{
    [CustomItem(ItemType.Adrenaline)]
    public class Calyxanide : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.Adrenaline;
        public override uint Id { get; set; } = 49;
        public override string Name { get; set; } = "<color=#6600CC>Calyxanide</color>";
        public override string Description { get; set; } = "A powerful drug that cures you of a Husk Infection";
        public override float Weight { get; set; } = 1;
        public string CalyxanideUseText { get; set; } = "<color=green><size=30>You feel a strange sensation in your throat, but it quickly passes.</size></color>";
        public bool UseHints { get; set; } = false;
        public float TextDisplayTime { get; set; } = 10f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside939Cryo,
                },
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside079Armory,
                },
                new ()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside049Armory
                }
            },
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 191);

        protected override void SubscribeEvents()
        {
            Player.UsingItem += OnUsingItem;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItem -= OnUsingItem;
            base.UnsubscribeEvents();
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            ev.IsAllowed = false;
            ev.Player.RemoveHeldItem();
            Log.Debug($"VVUP Custom Items: Calyxanide, {ev.Player.Nickname} used Calyxanide. Removing Husk Infection if they have it and displaying text.");
            if (HuskInfectionEventHandlers.PlayersWithHuskInfection.ContainsKey(ev.Player))
            {
               if (UseHints)
                   ev.Player.ShowHint(CalyxanideUseText, TextDisplayTime);
               else
                   ev.Player.Broadcast((ushort)TextDisplayTime, CalyxanideUseText, shouldClearPrevious: true);
               HuskInfectionEventHandlers.PlayersWithHuskInfection.Remove(ev.Player);
               HuskInfectionEventHandlers.PlayersMutedDueToHuskInfection.Remove(ev.Player);
            }
        }
    }
}