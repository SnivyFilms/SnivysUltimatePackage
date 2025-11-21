using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using UnityEngine;

namespace VVUP.PostNukeEnhancements
{
    public class EventHandlers
    {
        public Plugin Plugin;
        public EventHandlers(Plugin plugin) => Plugin = plugin;

        private static CoroutineHandle _lightsHandle;
        private static CoroutineHandle _radiationHandle;
        public void OnWaitingForPlayers()
        {
            Log.Debug("VVUP Post Nuke Enhancements: Killing Coroutines for lights and radiation.");
            Timing.KillCoroutines(_lightsHandle);
            Timing.KillCoroutines(_radiationHandle);
        }
        
        public void OnNukeDetonated()
        {
            Timing.KillCoroutines(_lightsHandle);
            Timing.KillCoroutines(_radiationHandle);
            _lightsHandle = Timing.RunCoroutine(LightCoroutine());
            _radiationHandle = Timing.RunCoroutine(RadiationCoroutine());
            Timing.CallDelayed(0.5f, () =>
            {
                var surfaceGate = DoorType.SurfaceGate;
                Door door = Door.Get(surfaceGate);
                if (Plugin.Config.SurfaceGateClose)
                {
                    Log.Debug("VVUP Post Nuke Enhancements: Closing Surface Gate Door.");
                    door.IsOpen = false;
                }
                if (Plugin.Config.SurfaceGateUnlock && door.IsLocked)
                {
                    Log.Debug("VVUP Post Nuke Enhancements: Unlocking Surface Gate Door.");
                    door.ChangeLock(DoorLockType.None);
                }
            });
        }
        
        private IEnumerator<float> LightCoroutine()
        {
            Log.Debug("VVUP Post Nuke Enhancements: Starting LightCoroutine.");
            var surfaceRooms = Room.List.Where(room => room.Type == RoomType.Surface).ToList();
            if (Plugin.Config.SurfaceBlackout)
            {
                surfaceRooms.ForEach(room => room.TurnOffLights(Plugin.Config.SurfaceBlackoutDuration));
                Log.Debug($"VVUP Post Nuke Enhancements: Surface lights blacked out for {Plugin.Config.SurfaceBlackoutDuration} seconds.");
                yield return Timing.WaitForSeconds(Plugin.Config.SurfaceBlackoutDuration);
            }

            if (!Plugin.Config.SurfaceColorInstant)
            {
                Color targetColor = Plugin.Config.SurfaceEndColor;
                Color currentColor = Plugin.Config.SurfaceStartColor;
                if (targetColor.r < 0 || targetColor.g < 0 || targetColor.b < 0 || targetColor.a < 0)
                {
                    surfaceRooms.ForEach(room => room.ResetColor());
                    Log.Debug("VVUP Post Nuke Enhancements: Resetting Surface Color to default.");
                }

                while (!ColorEquals(currentColor, targetColor))
                {
                    currentColor = Color.Lerp(currentColor, targetColor, Plugin.Config.SurfaceColorChangeAmount);
            
                    if (Mathf.Abs(currentColor.r - targetColor.r) < 0.01f &&
                        Mathf.Abs(currentColor.g - targetColor.g) < 0.01f &&
                        Mathf.Abs(currentColor.b - targetColor.b) < 0.01f &&
                        Mathf.Abs(currentColor.a - targetColor.a) < 0.01f)
                    {
                        currentColor = targetColor;
                    }

                    surfaceRooms.ForEach(room => room.Color = currentColor);
                    Log.Debug($"VVUP Post Nuke Enhancements: Updating surface color to {currentColor.r}, {currentColor.g}, {currentColor.b}, {currentColor.a}, waiting for {Plugin.Config.SurfaceColorUpdateInterval} seconds.");
                    yield return Timing.WaitForSeconds(Plugin.Config.SurfaceColorUpdateInterval);
                }
                Log.Debug($"VVUP Post Nuke Enhancements: Surface color transition complete to {targetColor.r}, {targetColor.g}, {targetColor.b}, {targetColor.a}.");
                surfaceRooms.ForEach(room => room.Color = targetColor);
            }
            else
            {
                if (Plugin.Config.SurfaceEndColor.r < 0 || Plugin.Config.SurfaceEndColor.g < 0 || Plugin.Config.SurfaceEndColor.b < 0 || Plugin.Config.SurfaceEndColor.a < 0)
                {
                    surfaceRooms.ForEach(room => room.ResetColor());
                    Log.Debug("VVUP Post Nuke Enhancements: Resetting Surface Color to default.");
                }
                surfaceRooms.ForEach(room => room.Color = Plugin.Config.SurfaceEndColor);
                Log.Debug($"VVUP Post Nuke Enhancements: Setting surface color instantly to {Plugin.Config.SurfaceEndColor.r}, {Plugin.Config.SurfaceEndColor.g}, {Plugin.Config.SurfaceEndColor.b}, {Plugin.Config.SurfaceEndColor.a}");
            }
        }
        
        private IEnumerator<float> RadiationCoroutine()
        {
            Log.Debug("VVUP Post Nuke Enhancements: Starting RadiationCoroutine.");
            Log.Debug($"VVUP Post Nuke Enhancements: Waiting for radiation delay of {Plugin.Config.RadiationDelay} seconds.");
            yield return Timing.WaitForSeconds(Plugin.Config.RadiationDelay);
            Log.Debug("VVUP Post Nuke Enhancements: Radiation started.");
            if (Plugin.Config.RadiationEnabled)
            {
                foreach (Player player in Player.List.Where(player => !player.IsDead))
                {
                    if (Plugin.Config.UseHints)
                        player.ShowHint(Plugin.Config.RadiationStartMessage, Plugin.Config.RadiationMessageDuration);
                    else
                        player.Broadcast((ushort)Plugin.Config.RadiationMessageDuration, Plugin.Config.RadiationStartMessage);
                    Log.Debug($"VVUP Post Nuke Enhancements: Notified {player.Nickname} about radiation start.");
                }
            }
            while (Plugin.Config.RadiationEnabled)
            {
                foreach (var player in Player.List)
                {
                    if (player.IsDead)
                    {
                        Log.Debug($"VVUP Post Nuke Enhancements: {player.Nickname} is dead, skipping radiation damage.");
                        continue;
                    }
                    if (Plugin.Config.RadiationImmuneRoles.Contains(player.Role.Type))
                    {
                        Log.Debug($"VVUP Post Nuke Enhancements: {player.Nickname} is immune to radiation due to their role ({player.Role.Type}).");
                        continue;
                    }
                            
                    float damage = Plugin.Config.RadiationDamage;
                    if (player.IsScp)
                    {
                        damage *= Plugin.Config.RadiationDamageScpMultiplier;
                    }
                    Log.Debug($"VVUP Post Nuke Enhancements: Applying {damage} radiation damage to {player.Nickname}.");
                    player.Hurt(damage, Plugin.Config.RadiationDeathMessage);
                    foreach (var effect in Plugin.Config.RadiationEffects)
                    {
                        Log.Debug($"VVUP Post Nuke Enhancements: Applying effect {effect.EffectType} with intensity {effect.Intensity} for duration {effect.Duration} to {player.Nickname}.");
                        player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration);
                    }
                }
                yield return Timing.WaitForSeconds(Plugin.Config.RadiationIntervalTime);
            }
            Log.Debug("VVUP Post Nuke Enhancements: RadiationCoroutine ended as radiation is disabled.");
        }

        private static bool ColorEquals(Color currentColor, Color targetColor)
        {
            return Mathf.Approximately(currentColor.r, targetColor.r) 
                   && Mathf.Approximately(currentColor.g, targetColor.g) 
                   && Mathf.Approximately(currentColor.b, targetColor.b) 
                   && Mathf.Approximately(currentColor.a, targetColor.a);
        }
    }
}