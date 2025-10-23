using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VVUP.HuskInfection.EventHandlers
{
    public class HuskInfectionEventHandlers
    {
        private static CoroutineHandle _huskHandle;
        public static Dictionary<Player, RoleTypeId> PlayersWithHuskInfection = new Dictionary<Player, RoleTypeId>();
        public static List<Player>PlayersMutedDueToHuskInfection = new List<Player>();

        public Plugin Plugin;
        public HuskInfectionEventHandlers(Plugin plugin) => Plugin = plugin;
        public HuskInfectionEventHandlers(Player player, float stageOneDelay, float stageTwoDelay, 
            string infectionText, bool useHints, float textDuration, uint huskZombieCustomRoleId, string huskTakeOverDeathReason)
        {
            if (Plugin.Instance.HuskInfectionEventHandlers == null)
            {
                Log.Debug("VVUP Husk Infection: Husk Infection Event Handlers is null, returning.");
                return;
            }
            if (PlayersWithHuskInfection.ContainsKey(player))
            {
                Log.Debug($"VVUP Husk Infection: {player.Nickname} is already infected with Husk Infection, returning.");
                return;
            }
            PlayersWithHuskInfection.Add(player, player.Role);
            Log.Debug($"VVUP Husk Infection: {player.Nickname} has been infected with Husk Infection, starting coroutine.");
            _huskHandle = Timing.RunCoroutine(HuskInfectionCoroutine(player, stageOneDelay, stageTwoDelay, 
                infectionText, useHints, textDuration, huskZombieCustomRoleId, huskTakeOverDeathReason));
        }

        public static IEnumerator<float> HuskInfectionCoroutine(Player player, float stageOneDelay, float stageTwoDelay, 
            string infectionText, bool useHints, float textDuration, uint huskZombieCustomRoleId, string huskTakeOverDeathReason)
        {
            yield return Timing.WaitForSeconds(stageOneDelay);
            if (!player.IsAlive || PlayersWithHuskInfection.ContainsKey(player) && 
                PlayersWithHuskInfection.TryGetValue(player, out RoleTypeId roleStage1) && roleStage1 != player.Role)
            {
                Log.Debug($"VVUP Husk Infection: {player.Nickname} is no longer alive or has changed role before stage 1, clearing info.");
                PlayersWithHuskInfection.Remove(player);
                yield break;
            }

            if (useHints)
            {
                Log.Debug($"VVUP Husk Infection: {player.Nickname} has reached stage 1 with Husk Infection, showing infection text by hint.");
                player.ShowHint(infectionText, textDuration);
            }
            else
            {
                Log.Debug($"VVUP Husk Infection: {player.Nickname} has reached stage 1 with Husk Infection, showing infection text by broadcast.");
                player.Broadcast((ushort)textDuration, infectionText, shouldClearPrevious: true);
            }
            PlayersMutedDueToHuskInfection.Add(player);
            yield return Timing.WaitForSeconds(stageTwoDelay);
            if (!player.IsConnected)
            {
                Log.Debug($"VVUP Husk Infection: {player.Nickname} disconnected before stage 2, clearing info.");
                PlayersWithHuskInfection.Remove(player);
                PlayersMutedDueToHuskInfection.Remove(player);
                yield break;
            }
            if (player.IsAlive && PlayersWithHuskInfection.ContainsKey(player) &&
                PlayersWithHuskInfection.TryGetValue(player, out RoleTypeId roleStage2) && roleStage2 == player.Role
                && PlayersMutedDueToHuskInfection.Contains(player))
            {
                Vector3 huskedPlayersPos = player.Position;
                Player newPlayer = GetHuskPlayerToTakeOver();
                if (newPlayer == null)
                {
                    newPlayer = player;
                }
                Log.Debug($"VVUP Husk Infection: Killing {player.Nickname} to be taken over by {newPlayer.Nickname} to become a Husk at {huskedPlayersPos}");
                player.Kill(huskTakeOverDeathReason);
                yield return Timing.WaitForSeconds(0.5f);
                Log.Debug($"VVUP Husk Infection: {newPlayer.Nickname} is now a Husk, setting role to Husk, setting position to {huskedPlayersPos}");
                CustomRole.Get(huskZombieCustomRoleId)?.AddRole(newPlayer);
                newPlayer.Position = huskedPlayersPos;
            }
            else if (!player.IsAlive && PlayersWithHuskInfection.ContainsKey(player) && !PlayersMutedDueToHuskInfection.Contains(player))
            {
                Log.Debug($"VVUP Husk Infection: {player.Nickname} is now a husk, removing Husk Infection, setting role to Husk.");
                CustomRole.Get(huskZombieCustomRoleId)?.AddRole(player);
            }
            
            PlayersWithHuskInfection.Remove(player);
            PlayersMutedDueToHuskInfection.Remove(player);
        }

        public void OnVoiceChatting(VoiceChattingEventArgs ev)
        {
            if (!PlayersMutedDueToHuskInfection.Contains(ev.Player))
            {
                ev.IsAllowed = true;
            }
            else if (!PlayersWithHuskInfection.ContainsKey(ev.Player))
            {
                Log.Debug(
                    $"VVUP Husk Infection: Found inconsistent state for {ev.Player.Nickname}, removing mute effect");
                PlayersMutedDueToHuskInfection.Remove(ev.Player);
                ev.IsAllowed = true;
            }
            else
            {
                Log.Debug(
                    $"VVUP Husk Infection: {ev.Player.Nickname} is muted due to Husk Infection, preventing voice chat.");
                ev.IsAllowed = false;
            }
        }

        public void OnRoleChange(ChangingRoleEventArgs ev)
        {
            if (PlayersWithHuskInfection.ContainsKey(ev.Player) && PlayersMutedDueToHuskInfection.Contains(ev.Player))
            {
                Log.Debug($"VVUP Husk Infection: {ev.Player.Nickname} is changing role while infected with Husk Infection & Muted, removing muted effect.");
                PlayersMutedDueToHuskInfection.Remove(ev.Player);
            }
            else if (PlayersWithHuskInfection.ContainsKey(ev.Player) && !PlayersMutedDueToHuskInfection.Contains(ev.Player))
            {
                Log.Debug($"VVUP Husk Infection: {ev.Player.Nickname} is changing role while infected with Husk Infection, but not muted, removing Husk Infection.");
                PlayersWithHuskInfection.Remove(ev.Player);
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (_huskHandle.IsRunning)
            {
                Log.Debug("VVUP Husk Infection: Husk Infection Coroutine is still running, killing it.");
                Timing.KillCoroutines(_huskHandle);
            }
            Log.Debug("VVUP Husk Infection: Clearing Husk Infection data.");
            PlayersWithHuskInfection.Clear();
            PlayersMutedDueToHuskInfection.Clear();
        }
        
        public void OnWaitingForPlayers()
        {
            if (_huskHandle.IsRunning)
            {
                Log.Debug("VVUP Husk Infection: Husk Infection Coroutine is still running, killing it.");
                Timing.KillCoroutines(_huskHandle);
            }
            Log.Debug("VVUP Husk Infection: Clearing Husk Infection data.");
            PlayersWithHuskInfection.Clear();
            PlayersMutedDueToHuskInfection.Clear();
        }

        private static Player GetHuskPlayerToTakeOver()
        {
            return Base.GetRandomSpectator.GetSpectator();
        }
    }
}