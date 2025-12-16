using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using LightContainmentZoneDecontamination;
using MEC;
using PlayerRoles;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Map = Exiled.API.Features.Map;
using Round = Exiled.API.Features.Round;
using Warhead = Exiled.API.Features.Warhead;

namespace VVUP.OperationCrossfireServerEvent
{
    public class OperationCrossfireEventHandlers
    {
        private static OperationCrossfireConfig _config;
        public static bool OcfStarted;
        private static OperationCrossfireEventHandlers _instance;
        
        // Objectives
        public static bool _scp914LockdownOverridden = false;
        public static bool _scientistsEscorted = false;
        public static bool _prototypeDeviceRefined = false;
        private static bool _unarmedScientistsCanBeKilled = false;
        
        // Player Tracking
        public static List<PlayerAPI> _mtfPlayers = new List<PlayerAPI>();
        public static List<PlayerAPI> _scientistPlayers = new List<PlayerAPI>();
        public static List<PlayerAPI> _classDPlayers = new List<PlayerAPI>();
        
        public static List<PlayerAPI> _playersSpectating = new List<PlayerAPI>();
        
        public static CoroutineHandle _ocfCoroutine;
        
        public OperationCrossfireEventHandlers()
        {
            if (OcfStarted)
            {
                Log.Debug($"VVUP Custom Events: Operation Crossfire: Event is already running");
                return;
            }

            OcfStarted = true;
            ServerEvents.Plugin.ActiveEvent += 1;
            _instance = this;

            _config = Plugin.Instance.Config;
            
            LabApi.Features.Wrappers.Cassie.Message(_config.StartEventCassieMessage, _config.StartEventCassieText);

            foreach (PlayerAPI player in PlayerAPI.List)
            {
                Log.Debug($"VVUP Custom Events: Operation Crossfire: Killing {player.Nickname}");
                player.Role.Set(RoleTypeId.Spectator);
            }

            var allPlayers = PlayerAPI.List.Where(p => p.Role != RoleTypeId.Overwatch);

            var playerEnumerable = allPlayers as PlayerAPI[] ?? allPlayers.ToArray();
            int total = playerEnumerable.Count();
            int mtfCount = (int)Math.Round(total * _config.MtfRatio);
            int scientistCount = (int)Math.Round(total * _config.ScientistRatio);
            int classDCount = total - mtfCount - scientistCount;
            
            Log.Debug("VVUP Custom Events: Operation Crossfire: Locking Round");
            Round.IsLocked = true;
            Log.Debug("VVUP Custom Events: Operation Crossfire: Locking Warhead");
            Warhead.IsLocked = true;
            Timing.CallDelayed(_config.EventDuration - 90, () =>
            {
                Log.Debug("VVUP Custom Events: Operation Crossfire: Starting Warhead");
                Warhead.IsLocked = false;
                Warhead.Start();
                Warhead.IsLocked = true;
            });
            Map.IsDecontaminationEnabled = false;
            
            DecontaminationController.Singleton.DecontaminationOverride =
                DecontaminationController.DecontaminationStatus.Disabled;
            
            // Event Handlers Setup
            PlayerEvent.Verified += OnPlayerJoinOcf;
            PlayerEvent.Died += OnPlayerDiedOcf;
            PlayerEvent.Left += OnPlayerLeaveOcf;
            PlayerEvent.InteractingDoor += OnDoorInteractOcf;
            PlayerEvent.Hurting += OnHurtingOcf;

            // Spawn Basic Keycard
            var customKeycardBasic = CustomItem.Get(_config.PrototypeKeycardBasicId);
            if (customKeycardBasic != null && customKeycardBasic.SpawnProperties != null)
            {
                var spawnPoints = customKeycardBasic.SpawnProperties.DynamicSpawnPoints;
                var selected = spawnPoints[Base.GetRandomNumber.GetRandomInt(spawnPoints.Count)];
                var position = selected.Location.GetPosition();
                customKeycardBasic.Spawn(position);
                Log.Debug(
                    $"VVUP Custom Events: Operation Crossfire: Spawned {customKeycardBasic.Name} at {position}");
            }

            // Player Setup
            Timing.CallDelayed(0.5f, () =>
            {
                var shuffledPlayers = playerEnumerable.OrderBy(_ => Guid.NewGuid()).ToArray(); // Shuffle players to randomize assignments

                int assignedPlayers = 0;

                for (int i = 0; i < mtfCount && assignedPlayers < shuffledPlayers.Length; i++, assignedPlayers++)
                {
                    Log.Debug($"VVUP Custom Events: Operation Crossfire: Adding {shuffledPlayers[assignedPlayers].Nickname} to MTF side");
                    shuffledPlayers[assignedPlayers].Role.Set(RoleTypeId.NtfSergeant);
                    _mtfPlayers.Add(shuffledPlayers[assignedPlayers]);
                    string mtfObjective = $"{_config.MtfScientistObjective1}\n{_config.MtfScientistObjective2}\n{_config.MtfObjective3}";
                    shuffledPlayers[assignedPlayers].Broadcast((ushort)_config.StartingBroadcastTime, mtfObjective);
                }

                for (int i = 0; i < scientistCount && assignedPlayers < shuffledPlayers.Length; i++, assignedPlayers++)
                {
                    Log.Debug($"VVUP Custom Events: Operation Crossfire: Adding {shuffledPlayers[assignedPlayers].Nickname} to Scientist side");
                    shuffledPlayers[assignedPlayers].Role.Set(RoleTypeId.Scientist);
                    _scientistPlayers.Add(shuffledPlayers[assignedPlayers]);
                    string scientistObjective = $"{_config.MtfScientistObjective1}\n{_config.MtfScientistObjective2}\n{_config.ScientistObjective3}";
                    shuffledPlayers[assignedPlayers].Broadcast((ushort)_config.StartingBroadcastTime, scientistObjective);
                }

                for (int i = 0; i < classDCount && assignedPlayers < shuffledPlayers.Length; i++, assignedPlayers++)
                {
                    Log.Debug($"VVUP Custom Events: Operation Crossfire: Adding {shuffledPlayers[assignedPlayers].Nickname} to D-Class side");
                    shuffledPlayers[assignedPlayers].Role.Set(RoleTypeId.ClassD);
                    _classDPlayers.Add(shuffledPlayers[assignedPlayers]);
                    string dClassObjective = $"{_config.ClassDObjective1}\n{_config.ClassDObjective2}";
                    shuffledPlayers[assignedPlayers].Broadcast((ushort)_config.StartingBroadcastTime, dClassObjective);
                    shuffledPlayers[assignedPlayers].AddAmmo(AmmoType.Ammo12Gauge, 24);
                    shuffledPlayers[assignedPlayers].AddAmmo(AmmoType.Ammo44Cal, 24);
                    shuffledPlayers[assignedPlayers].AddAmmo(AmmoType.Nato9, 40);
                    shuffledPlayers[assignedPlayers].AddAmmo(AmmoType.Nato556, 40);
                    shuffledPlayers[assignedPlayers].AddAmmo(AmmoType.Nato762, 40);
                    shuffledPlayers[assignedPlayers].AddItem(_config.ClassDKeycard);
                    shuffledPlayers[assignedPlayers]
                        .AddItem(_config.ClassDFirearms[Base.GetRandomNumber.GetRandomInt(_config.ClassDFirearms.Count)]);
                }
                
                // Start the Routine Proper
                _ocfCoroutine = Timing.RunCoroutine(OperationCrossfireTiming());
            });
        }

        public IEnumerator<float> OperationCrossfireTiming()
        {
            for (;;)
            {
                int totalScientists = _scientistPlayers.Count;
                if (totalScientists > 0)
                {
                    int escapedScientists = _scientistPlayers.Count(p =>
                        p.Role == RoleTypeId.Scientist && p.Zone == ZoneType.Surface);

                    float percentEscaped = (float)escapedScientists / totalScientists;
                    if (percentEscaped >= _config.ScientistPercentageRequiredToWin)
                    {
                        _scientistsEscorted = true;
                    }
                    int mtfAlive = _mtfPlayers.Count(p => p.Role.Team == Team.FoundationForces);
                    float percentMtfAlive = (float)mtfAlive / _mtfPlayers.Count;
                    if (_prototypeDeviceRefined && !_unarmedScientistsCanBeKilled)
                    {
                        Log.Debug("VVUP Custom Events: Operation Crossfire: Allowing Unarmed Scientists to be killed now");
                        _unarmedScientistsCanBeKilled = true;
                        foreach (PlayerAPI player in _classDPlayers)
                        {
                            Log.Debug($"VVUP Custom Events: Sending {player.Nickname} the message that Scientists are now targets");
                            player.Broadcast((ushort)_config.EndOfRoundTime, _config.ClassDScientistsNowAreTargets);
                        }
                    }
                    if (_prototypeDeviceRefined && _scientistsEscorted && _scp914LockdownOverridden &&
                        percentMtfAlive >= _config.MtfPercentageRequiredToWin)
                    {
                        Map.Broadcast((ushort)_config.EndOfRoundTime, _config.MtfScientistWinMessage);
                        yield return Timing.WaitForSeconds(_config.EndOfRoundTime);
                        EndEvent();
                        yield break;
                    }
                    if (totalScientists - escapedScientists <= 0 && percentMtfAlive < _config.MtfPercentageRequiredToWin)
                    {
                        Map.Broadcast((ushort)_config.EndOfRoundTime, _config.ClassDWinMessage);
                        yield return Timing.WaitForSeconds(_config.EndOfRoundTime);
                        EndEvent();
                        yield break;
                    }
                    if (Warhead.IsDetonated)
                    {
                        Map.Broadcast((ushort)_config.EndOfRoundTime, _config.TieMessage);
                        yield return Timing.WaitForSeconds(_config.EndOfRoundTime);
                        EndEvent();
                        yield break;
                    }
                }
                yield return Timing.WaitForSeconds(_config.CheckForEventsInterval);
            }
        }

        public static void EndEvent()
        {
            if (!OcfStarted) return;
            Log.Debug("VVUP Custom Events: Operation Crossfire: Ending event");
            OcfStarted = false;
            Timing.KillCoroutines(_ocfCoroutine);
            PlayerEvent.Verified -= _instance.OnPlayerJoinOcf;
            PlayerEvent.Died -= _instance.OnPlayerDiedOcf;
            PlayerEvent.Left -= _instance.OnPlayerLeaveOcf;
            PlayerEvent.InteractingDoor -= _instance.OnDoorInteractOcf;
            PlayerEvent.Hurting -= _instance.OnHurtingOcf;
            foreach (PlayerAPI player in PlayerAPI.List)
            {
                Log.Debug($"VVUP Custom Events: Operation Crossfire: Killing {player.Nickname}");
                if (player.Role == RoleTypeId.Overwatch && _playersSpectating!.Contains(player))
                    continue;
                player.Role.Set(RoleTypeId.Tutorial);
            }
            foreach (PlayerAPI player in _playersSpectating.ToList())
                _playersSpectating.Remove(player);
            foreach (PlayerAPI player in _classDPlayers.ToList())
                _classDPlayers.Remove(player);
            foreach (PlayerAPI player in _mtfPlayers.ToList())
                _mtfPlayers.Remove(player);
            foreach (PlayerAPI player in _scientistPlayers.ToList())
                _scientistPlayers.Remove(player);

            ServerEvents.Plugin.ActiveEvent -= 1;
            if (!Warhead.IsDetonated)
            {
                Warhead.DetonationTimer = 90;
                Warhead.IsLocked = false;
                Warhead.Stop();
            }
        }
        
        public void OnPlayerJoinOcf(VerifiedEventArgs ev)
        {
            Log.Debug($"VVUP Custom Events: Operation Crossfire: Player {ev.Player.Nickname} has joined the server, setting them to Overwatch");
            ev.Player.Role.Set(RoleTypeId.Overwatch);
            ev.Player.Broadcast((ushort)Plugin.Instance.Config.PlayerConnectDuringEventMessageDisplayDuration, Plugin.Instance.Config.PlayerConnectDuringEventMessage);
            _playersSpectating.Add(ev.Player);
        }

        public void OnPlayerDiedOcf(DiedEventArgs ev)
        {
            if (!OcfStarted) return;
            Log.Debug($"VVUP Custom Events: Operation Crossfire: Player {ev.Player.Nickname} has died, setting them to Overwatch");
            Timing.CallDelayed(0.5f, () => ev.Player.Role.Set(RoleTypeId.Overwatch));
            _playersSpectating.Add(ev.Player);
        }
        
        public void OnPlayerLeaveOcf(LeftEventArgs ev)
        {
            if (!OcfStarted) return;
            Log.Debug($"VVUP Custom Events: Operation Crossfire: Player {ev.Player.Nickname} has left the server, removing them from the list");
            if (_playersSpectating.Contains(ev.Player))
                _playersSpectating.Remove(ev.Player);
        }

        public void OnDoorInteractOcf(InteractingDoorEventArgs ev)
        {
            if (!OcfStarted) return;
            if (ev.Door.Type == DoorType.Scp914Gate
                && ev.Player.CurrentItem != null
                && CustomItem.TryGet(ev.Player.CurrentItem, out var customItem)
                && customItem != null
                && customItem.Id == Plugin.Instance.Config.PrototypeKeycardBasicId
                && (_scientistPlayers.Contains(ev.Player) || _mtfPlayers.Contains(ev.Player))
                && !_scp914LockdownOverridden)
            {
                ev.Door.IsOpen = true;
                _scp914LockdownOverridden = true;
                Log.Debug($"VVUP Custom Events: Operation Crossfire: Player {ev.Player.Nickname} has opened SCP-914, overriding the lockdown of SCP-914");
            }
        }

        public void OnHurtingOcf(HurtingEventArgs ev)
        {
            if (!OcfStarted) return;
            if (!_scp914LockdownOverridden && ev.Attacker.Role == RoleTypeId.ClassD && ev.Player.Role == RoleTypeId.Scientist)
            {
                foreach (var item in ev.Player.Items)
                {
                    if (item.Type is ItemType.GunCOM15 or ItemType.GunCOM18 or ItemType.GunE11SR or
                        ItemType.GunCrossvec or ItemType.GunFSP9 or ItemType.GunLogicer or
                        ItemType.GunRevolver or ItemType.GunAK or ItemType.GunShotgun or ItemType.GrenadeFlash or 
                        ItemType.GrenadeHE or ItemType.SCP018 or ItemType.GunA7 or ItemType.GunSCP127 or ItemType.ParticleDisruptor or 
                        ItemType.Jailbird or ItemType.MicroHID)
                    {
                        break;
                    }
                    ev.Attacker.Broadcast((ushort)Plugin.Instance.Config.EndOfRoundTime, Plugin.Instance.Config.ClassDScientistHostagesUnableToHarm);
                    ev.IsAllowed = false;
                }
            }
        }
    }
}