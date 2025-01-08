﻿using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SnivysUltimatePackage.Configs.ServerEventsConfigs;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace SnivysUltimatePackage.EventHandlers.ServerEventsEventHandlers
{
    public class SnowballsVsScpsEventHandlers
    {
        private static SnowballsVsScpsConfig _config;
        private static bool _svsStarted;
        private static CoroutineHandle _snowballsVsScpsCoroutine;
        private List<Player> _players;
        private Player _randomPlayer;
        public static List<Player> PlayersInOverwatchFromEvent = new List<Player>();

        public SnowballsVsScpsEventHandlers()
        {
            Log.Debug("VVUP Server Events: Snowballs Vs Scps, checking if the event has already started");
            if (_svsStarted)
                return;
            Log.Debug("VVUP Server Events: Snowballs Vs Scps, Starting event");
            _config = Plugin.Instance.Config.ServerEventsMasterConfig.SnowballsVsScpsConfig;
            Plugin.ActiveEvent += 1;
            _svsStarted = true;
            PlayerEvent.Dying += Plugin.Instance.ServerEventsMainEventHandler.OnDyingSvs;
            Random random = new Random();
            _players = Player.List.ToList();
            _randomPlayer = _players[random.Next(0, _players.Count)];
            _randomPlayer.Role.Set(_config.ScpRoles[random.Next(0, _config.ScpRoles.Count)]);
            Log.Debug($"VVUP Server Events: Snowballs Vs Scps, random player is {_randomPlayer.Nickname} and their role is {_randomPlayer.Role}");
            foreach (var player in _players)
            {
                if (player != _randomPlayer)
                {
                    player.Role.Set(_config.HumanRoles[random.Next(0, _config.HumanRoles.Count)], SpawnReason.ForceClass, RoleSpawnFlags.None);
                    Timing.CallDelayed(0.25f, () =>
                        player.ClearInventory());
                    Log.Debug($"VVUP Server Events: Snowballs Vs Scps, setting {player.Nickname} to {player.Role} and clearing inventory");
                }
            }

            Log.Debug("VVUP Server Events: Snowballs Vs Scps, Starting Coroutine");
            _snowballsVsScpsCoroutine = Timing.RunCoroutine(SnowballRefillRoutine());
        }

        private IEnumerator<float> SnowballRefillRoutine()
        {
            if (!_svsStarted)
                yield break;

            for (;;)
            {
                foreach (Player player in _players)
                {
                    if (player != _randomPlayer || !player.IsDead || !player.IsOverwatchEnabled)
                    {
                        Log.Debug($"VVUP Server Events: Snowballs Vs Scps, adding snowballs to {player.Nickname} until their inventory is full");
                        while (!player.IsInventoryFull)
                        {
                            player.AddItem(ItemType.Snowball);
                            Log.Debug($"VVUP Server Events: Snowballs Vs Scps, {player.Nickname} has {player.CountItem(ItemType.Snowball)} in their inventory");
                        }
                    }
                }
                Log.Debug($"VVUP Server Events: Snowballs Vs Scps, waiting for {_config.SnowballRefillCycle} seconds");
                yield return Timing.WaitForSeconds(_config.SnowballRefillCycle);
            }
        }

        public static void EndEvent()
        {
            if (!_svsStarted) return;
            _svsStarted = false;
            Log.Debug("VVUP Server Events: Snowballs Vs Scps, Killing main Coroutine Handle for Snowballs Vs Scps");
            Timing.KillCoroutines(_snowballsVsScpsCoroutine);
            Plugin.ActiveEvent -= 1;
            PlayerEvent.Dying -= Plugin.Instance.ServerEventsMainEventHandler.OnDyingSvs;
            foreach (Player player in Player.List)
            {
                if (PlayersInOverwatchFromEvent.Contains(player))
                {
                    player.Role.Set(RoleTypeId.Spectator);
                    Log.Debug($"VVUP Server Events: Snowballs Vs Scps, Setting {player.Nickname} to spectator as the event is over.");
                }
            }
            Log.Debug("VVUP Server Events: Snowballs Vs Scps, Clearing Players in Overwatch from Event List");
            PlayersInOverwatchFromEvent.Clear();
        }
    }
}