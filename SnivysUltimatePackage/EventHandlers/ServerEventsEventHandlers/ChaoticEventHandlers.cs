﻿using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Hazards;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using LightContainmentZoneDecontamination;
using MEC;
using PlayerRoles;
using SnivysUltimatePackage.Configs.ServerEventsConfigs;
using UnityEngine;
using Cassie = Exiled.API.Features.Cassie;
using Item = Exiled.API.Features.Items.Item;
using Map = Exiled.API.Features.Map;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Random = System.Random;
using Tesla = Exiled.API.Features.TeslaGate;
using Warhead = Exiled.API.Features.Warhead;

namespace SnivysUltimatePackage.EventHandlers.ServerEventsEventHandlers;
public class ChaoticEventHandlers
{
    private static CoroutineHandle _choaticHandle;
    private static CoroutineHandle _fakeWarheadHandle;
    private static CoroutineHandle _rapidFireTeslas;
    private static CoroutineHandle _routerKickingSimulator;
    private static ChaoticConfig _config;
    private static bool _ceStarted;
    private static bool _ceMedicalItemEvent;
    private static bool _ceFakeWarheadEvent;
    private static bool _ceRapidFireTeslas;
    private static bool _ceRouterKickingSimulator;
    private static float _previousWarheadTime;
    private static double _previousDecomTime;
    public ChaoticEventHandlers()
    {
        Log.Debug("Checking if Chaotic Event has already started");
        if (_ceStarted) return;
        _config = Plugin.Instance.Config.ServerEventsMasterConfig.ChaoticConfig;
        Plugin.ActiveEvent += 1;
        _ceStarted = true;
        Cassie.MessageTranslated(_config.StartEventCassieMessage, _config.StartEventCassieText);
        _choaticHandle = Timing.RunCoroutine(ChaoticTiming());
    }

    private static IEnumerator<float> ChaoticTiming()
    {
        Random random = new Random();
        if (!_ceStarted)
            yield break;
        for (;;)
        {
            float chaoticEventCycle = _config.TimeForChaosEvent;
            int chaosRandomNumber = random.Next(minValue:1, maxValue:22);
            Log.Debug(chaosRandomNumber);
            if (_config.ChaosEventEndsOtherEvents)
            {
                Log.Debug("Event ends other events active, ending other events.");
                BlackoutEventHandlers.EndEvent();
                FreezingTemperaturesEventHandlers.EndEvent();
                PeanutHydraEventHandlers.EndEvent();
                PeanutInfectionEventHandlers.EndEvent();
                ShortEventHandlers.EndEvent();
                VariableLightsEventHandlers.EndEvent();
                NameRedactedEventHandlers.EndEvent();
            }
            switch (chaosRandomNumber)
            {
                // Item Steal
                case 1:
                    if (_config.ItemStealEvent)
                    {
                        Log.Debug("Item Steal Chaos Event is active, running code");
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            Log.Debug("Checking if players aren't a SCP or is dead");
                            if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                            {
                                Log.Debug($"Showing Broadcast to {player} saying their items got stolen");
                                player.Broadcast(new Exiled.API.Features.Broadcast(_config.ItemStolenText,
                                    (ushort)_config.BroadcastDisplayTime));
                                Log.Debug($"Clearing Items from {player}");
                                player.ClearItems();
                            }
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Item Steal Chaos Event disabled");
                    }

                    break;
                // Give Random Item
                case 2:
                    if (_config.GiveRandomItemEvent)
                    {
                        Log.Debug("Give Random Item Event is active, running code");
                        Log.Debug("Getting a list of both standard items and custom items");
                        ItemType[] standardItems = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray();
                        List<CustomItem> customItems = CustomItem.Registered.ToList();
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            Log.Debug("Checking if players aren't a SCP or is dead");
                            if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                            {
                                int randomItemGiveRng = random.Next(minValue: 1, maxValue: 2);
                                Log.Debug($"Deciding if {player} gets a Custom Item or a Regular Item");
                                if (_config.GiveRandomItemCustomitems)
                                {

                                    switch (randomItemGiveRng)
                                    {
                                        case 1:
                                            CustomItem randomCustomItem = customItems[random.Next(customItems.Count)];
                                            randomCustomItem.Give(player);
                                            Log.Debug($"{player} has received custom item id {randomCustomItem}");
                                            break;
                                        case 2:
                                            ItemType randomStandardItem =
                                                standardItems[random.Next(standardItems.Length)];
                                            player.AddItem(randomStandardItem);
                                            Log.Debug($"{player} has received item id {randomStandardItem}");
                                            break;
                                    }
                                }
                                
                                else
                                {
                                    ItemType randomStandardItem =
                                        standardItems[random.Next(standardItems.Length)];
                                    player.AddItem(randomStandardItem);
                                }
                                player.Broadcast(new Exiled.API.Features.Broadcast(_config.GiveRandomItemText,
                                    (ushort)_config.BroadcastDisplayTime));
                            }
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Give Random Item Event is disabled.");
                    }

                    break;
                // Random Teleport
                case 3:
                    if (_config.RandomTeleportEvent)
                    {
                        Log.Debug("Random Teleport Event is active, running code");
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            if (!_config.TeleportToFacilityAfterNuke && Warhead.IsDetonated)
                            {
                                Log.Debug($"Warhead has been detonated, teleporting {player} somewhere on Surface");
                                player.Teleport(Room.List.Where(r => r.Type is RoomType.Surface).GetRandomValue());
                            }
                            
                            else if (!_config.TeleportToLightAfterDecom && Map.DecontaminationState == DecontaminationState.Finish)
                            { 
                                Log.Debug($"Light has been decontaminated, teleporting {player} somewhere else in the facility");
                                player.Teleport(Room.List.Where(r => r.Zone is not ZoneType.LightContainment).GetRandomValue());
                            }
                            
                            else
                            {
                                Log.Debug($"Teleporting {player} randomly into the facility");
                                player.Teleport(Room.List.GetRandomValue());
                            }
                            player.Broadcast(new Exiled.API.Features.Broadcast(_config.RandomTeleportText, (ushort)_config.BroadcastDisplayTime));
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Random Teleport Event is disabled");
                    }

                    break;
                // Fake Auto Nuke
                case 4:
                    if (_config.FakeAutoNuke)
                    {
                        Log.Debug("Fake auto nuke event is active, running code");
                        if (!Warhead.IsDetonated || !Warhead.IsInProgress)
                        {
                            _ceFakeWarheadEvent = true;
                            Log.Debug("Saving previous warhead time in the event of the nuke being activated before the fake auto nuke");
                            if(_config.FakeAutoNukeRestoresOldTime)
                                _previousWarheadTime = Warhead.RealDetonationTimer;
                            Log.Debug("Starting warhead");
                            Warhead.Start();
                            Log.Debug("Checking if the Warhead is locked, if not lock it");
                            if (!Warhead.IsLocked)
                                Warhead.IsLocked = true;
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug($"Displaying the fake autonuke start message to {player}");
                                player.Broadcast(new Exiled.API.Features.Broadcast(_config.FakeAutoNukeStartText,
                                    (ushort)_config.BroadcastDisplayTime));
                            }

                            Log.Debug("Starting a Coroutine to track warhead timing");
                            _fakeWarheadHandle = Timing.RunCoroutine(WarheadTiming());
                        }
                        else
                            Log.Debug("Warhead is either detonated or is in progress, not running this event");
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Fake auto nuke event is disabled");
                    }

                    break;
                // Remove Weapons
                case 5:
                    if (_config.RemoveWeaponsEvent)
                    {
                        Log.Debug("Remove Weapons event is active, running code");
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                            {
                                foreach (var item in player.Items)
                                {
                                    Log.Debug($"Checking if {player} has a weapon");
                                    if (IsWeapon(item))
                                    {
                                        Log.Debug($"Taking away {player}'s {item}");
                                        player.RemoveItem(item);
                                        player.Broadcast(new Exiled.API.Features.Broadcast(_config.RemoveWeaponsText,
                                            (ushort)_config.BroadcastDisplayTime));
                                    }
                                }
                            }
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Remove Weapons Event is disabled");
                    }

                    break;
                // Give Weapons
                case 6:
                    if (_config.GiveRandomWeaponsEvent)
                    {
                        Log.Debug("Giving random weapons is active, running code");
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                            {
                                Log.Debug($"Checking if {player} is able to get a random weapon");
                                ItemType randomWeapon = WeaponTypes[random.Next(WeaponTypes.Length)];
                                if (_config.GiveRandomWeaponsToUnarmedPlayers)
                                {
                                    foreach (var item in player.Items)
                                    {
                                        Log.Debug($"Checking if {player} has a weapon");
                                        if (IsWeapon(item))
                                        {
                                            Log.Debug($"{player} has a weapon, not granting another");
                                            player.Broadcast(new Exiled.API.Features.Broadcast(_config.GiveRandomWeaponsTextFail,
                                                (ushort)_config.BroadcastDisplayTime));
                                        }
                                        
                                        else
                                        {
                                            if (_config.GiveAllRandomWeapons)
                                            {
                                                Log.Debug($"Giving {player} a random weapon");
                                                player.AddItem(randomWeapon);
                                            }
                                            
                                            else if (_config.GiveRandomWeaponsDefined == null)
                                            {
                                                Log.Warn("VVE Chaos Event: GiveAllRandomWeapons is false but the GiveRandomWeaponsDefined is empty! Falling back to any random weapon.");
                                                player.AddItem(randomWeapon);
                                            }
                                            
                                            else
                                            {
                                                Log.Debug($"Giving {player} a predefined weapon");
                                                player.AddItem(
                                                    _config.GiveRandomWeaponsDefined[
                                                        random.Next(_config.GiveRandomWeaponsDefined.Count)]);
                                            }

                                            player.Broadcast(new Exiled.API.Features.Broadcast(_config.GiveRandomWeaponsText,
                                                (ushort)_config.BroadcastDisplayTime));
                                        }
                                    }
                                }
                                
                                else
                                {
                                    if (player.IsInventoryFull)
                                    {
                                        Log.Debug($"{player}'s inventory is full, not giving any weapon");
                                        player.Broadcast(new Exiled.API.Features.Broadcast(_config.GiveRandomWeaponsTextFail,
                                            (ushort)_config.BroadcastDisplayTime));
                                    }
                                    
                                    else
                                    {
                                        if (_config.GiveAllRandomWeapons)
                                        {
                                            Log.Debug($"Giving {player} a random weapon");
                                            player.AddItem(randomWeapon);
                                        }
                                        
                                        else if (_config.GiveRandomWeaponsDefined == null)
                                        {
                                            Log.Warn("VVE Chaos Event: GiveAllRandomWeapons is false but the GiveRandomWeaponsDefined is empty! Falling back to any random weapon.");
                                            player.AddItem(randomWeapon);
                                        }
                                        
                                        else
                                        {
                                            Log.Debug($"Giving {player} a predefined weapon");
                                            player.AddItem(
                                                _config.GiveRandomWeaponsDefined[
                                                    random.Next(_config.GiveRandomWeaponsDefined.Count)]);
                                        }
                                        player.Broadcast(new Exiled.API.Features.Broadcast(_config.GiveRandomWeaponsText,
                                            (ushort)_config.BroadcastDisplayTime));
                                    }
                                }
                            }
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Giving random weapons event is disabled");
                    }

                    break;
                // Death Match
                case 7:
                    if (_config.DeathMatchEvent)
                    {
                        Log.Debug("Death Match Event is active, running code");
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            Log.Debug("Checking if SCPs should be affected");
                            if (!_config.DeathMatchEventAffectsSCPs)
                            {
                                Log.Debug("SCPs aren't affected");
                                if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                                {
                                    Log.Debug($"Setting {player}'s health to {_config.DeathMatchHealth}");
                                    player.Health = _config.DeathMatchHealth;
                                    player.Broadcast(new Exiled.API.Features.Broadcast(_config.DeathMatchText,
                                        (ushort)_config.BroadcastDisplayTime));
                                }
                            }
                            
                            else
                            {
                                Log.Debug("SCPs are affected");
                                Log.Debug($"Setting {player}'s health to {_config.DeathMatchHealth}");
                                player.Health = _config.DeathMatchHealth;
                                player.Broadcast(new Exiled.API.Features.Broadcast(_config.DeathMatchText,
                                    (ushort)_config.BroadcastDisplayTime));
                            }
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Death Match Event is disabled");
                    }

                    break;
                // Blackout
                case 8:
                    if (_config.ChaosEventEnablesOtherEvents)
                    {
                        Log.Debug("Chaos Event Enables other Events true, running Blackout Event");
                        var blackoutEventHandlers = new BlackoutEventHandlers();
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Chaos Event enabling other events is disabled");
                    }

                    break;
                // Freezing Temperatures
                case 9:
                    if (_config.ChaosEventEnablesOtherEvents)
                    {
                        Log.Debug("Chaos Event Enables other Events true, running Freezing Temperature Event");
                        var freezingTemperaturesEventHandlers = new FreezingTemperaturesEventHandlers();
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Chaos Event enabling other events is disabled");
                    }

                    break;
                // Peanut Hydra
                case 10:
                    if (_config.ChaosEventEnablesOtherEvents)
                    {
                        Log.Debug("Chaos Event Enables other Events true, running Peanut Hydra Event");
                        var peanutHydraEventHandlers = new PeanutHydraEventHandlers();
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Chaos Event enabling other events is disabled");
                    }

                    break;
                // Peanut Infection
                case 11:
                    if (_config.ChaosEventEnablesOtherEvents)
                    {
                        Log.Debug("Chaos Event Enables other Events true, running Peanut Infection Event");
                        var peanutInfectionEventHandlers = new PeanutInfectionEventHandlers();
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Chaos Event enabling other events is disabled");
                    }

                    break;
                // Short People Event
                case 12:
                    if (_config.ChaosEventEnablesOtherEvents)
                    {
                        Log.Debug("Chaos Event Enables other Events true, running Short People Event");
                        var shortEventHandlers = new ShortEventHandlers();
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Chaos Event enabling other events is disabled");
                    }

                    break;
                // Variable Lights
                case 13:
                    if (_config.ChaosEventEnablesOtherEvents)
                    {
                        Log.Debug("Chaos Event Enables other Events true, running Variable Lights Event");
                        var variableLightsEvent = new VariableLightsEventHandlers();
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Chaos Event enabling other events is disabled");
                    }

                    break;
                // FBI Open Up Event
                case 14:
                    if (_config.FBIOpenUpEvent)
                    {
                        Log.Debug("FBI Open Up Event active, running code");
                        Log.Debug("Getting a random non-foundation player");
                        PlayerAPI FBIOpenUpTarget = GetRandomPlayerFBI();
                        Log.Debug($"Showing {FBIOpenUpTarget} the warning message they are about to have the foundation teleport to them");
                        FBIOpenUpTarget.Broadcast(new Exiled.API.Features.Broadcast(_config.FBIOpenUpTargetText, (ushort)_config.BroadcastDisplayTime));
                        yield return Timing.WaitForSeconds(_config.FBITeleportTime);
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            if (player.Role.Team == Team.FoundationForces)
                            {
                                Log.Debug($"{player} is on the MTF, Showing warning text");
                                player.Broadcast(new Exiled.API.Features.Broadcast(_config.FBIOpenUpMTFText, (ushort)_config.BroadcastDisplayTime));
                                Log.Debug($"Teleporting {player} to {FBIOpenUpTarget}");
                                player.Teleport(FBIOpenUpTarget.Position);
                            }
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("FBI Open Up Event disabled");
                    }

                    break;
                // Grenade Feet
                case 15:
                    if (_config.GrenadeFeetEvent)
                    {
                        Log.Debug("Grenade Feet Event active, running code");
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            Log.Debug($"Grenade feet warning being shown to {player}");
                            player.Broadcast(new Exiled.API.Features.Broadcast(_config.GrenadeFeetText, (ushort) _config.BroadcastDisplayTime));
                        }

                        yield return Timing.WaitForSeconds(random.Next(minValue: 1, maxValue: 50));

                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            Log.Debug($"Spawning a grenade on {player}");
                            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                            if (_config.GrenadeFeetRandomFuse)
                                grenade.FuseTime = random.Next(minValue: 1, maxValue: 50);
                            else
                                grenade.FuseTime = _config.GrenadeFeetFuse;
                            grenade.SpawnActive(player.Position);
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Grenade Feet Event disabled");
                    }

                    break;
                // Unsafe Medical Items
                case 16:
                    if (_config.UnsafeMedicalItemsEvent)
                    {
                        Log.Debug("Unsafe medical items event active, running code");
                        Log.Debug("Activating Event Handlers for on using medical item");
                        PlayerEvent.UsingItemCompleted += Plugin.Instance.ServerEventsMainEventHandler.OnUsingMedicalItemCE;
                        _ceMedicalItemEvent = true;
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            if (player.Role.Team != Team.SCPs)
                            {
                                Log.Debug($"Displaying Unsafe Medical Items Warning to {player}");
                                player.Broadcast(new Exiled.API.Features.Broadcast(_config.UnsafeMedicalItemsText,
                                    (ushort)_config.BroadcastDisplayTime));
                            }
                        }
                        if (_config.UnsafeMedicalItemsUseRandomTime)
                            yield return Timing.WaitForSeconds(random.Next(minValue: 1, maxValue: 50));
                        else
                            yield return Timing.WaitForSeconds(_config.UnsafeMedicalItemsFixedTime);
                        Log.Debug("Disabling Event Handlers for on using medical item events");
                        PlayerEvent.UsingItemCompleted -= Plugin.Instance.ServerEventsMainEventHandler.OnUsingMedicalItemCE;
                        _ceMedicalItemEvent = false;
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            if (player.Role.Team != Team.SCPs)
                            {
                                Log.Debug($"Displaying Unsafe Medical Items Warning to {player}");
                                player.Broadcast(new Exiled.API.Features.Broadcast(_config.UnsafeMedicalItemsSafeToUseText,
                                    (ushort)_config.BroadcastDisplayTime));
                            }
                        }
                    }
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Unsafe medical Items Event disabled");
                    }

                    break;
                // Name Redacted
                case 17:
                    if (_config.ChaosEventEnablesOtherEvents)
                    {
                        Log.Debug("Chaos Event Enables other Events true, running Name Redacted Event");
                        var nameRedactedHandlers = new NameRedactedEventHandlers();
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Chaos Event enabling other events is disabled");
                    }

                    break;
                // Fakeout Respawn Announcements
                case 18:
                    if (_config.FakeoutRespawnAnnouncementsEvent)
                    {
                        Log.Debug("Fakeout Respawn Announcements active, running code");
                        float fakeoutRespawnRandom = random.Next(minValue:1, maxValue:4);
                        string cassieMessage = string.Empty;
                        string cassieText = string.Empty;
                        int scpCount = 0;
                        switch (fakeoutRespawnRandom)
                        {
                            case 1:
                                if (_config.FakeoutRespawnAnnouncementsMTFAllow)
                                    MtfFakeoutCassie(cassieMessage, cassieText, scpCount);
                                
                                break;
                            
                            case 2:
                                if (_config.FakeoutRespawnAnnouncementsUIUAllow)
                                {
                                    foreach (Player player in Player.List)
                                    {
                                        if (player.Role.Team == Team.SCPs)
                                            scpCount++;
                                    }

                                    if (scpCount == 0)
                                    {
                                        cassieMessage = _config.FakeoutRespawnAnnouncementsUIUSCPSDeadCassie;
                                        cassieText = _config.FakeoutRespawnAnnouncementsUIUSCPSDeadCassieText;
                                        int randomNatoLetter = random.Next(minValue: 1, maxValue: 26);
                                        int randomNatoNumber = random.Next(minValue: 2, maxValue: 20);
                                        cassieMessage = cassieMessage.Replace("{designation}", $"nato_{GetNatoLetter(randomNatoLetter)} {randomNatoNumber}");
                                        cassieText = cassieText.Replace("{designation}", GetNatoName(randomNatoLetter) + " " + random.Next(randomNatoNumber));
                                        Cassie.MessageTranslated(cassieMessage, cassieText);
                                    }
                                    else
                                    {
                                        cassieMessage = _config.FakeoutRespawnAnnouncementsUIUAliveSCPSCassie;
                                        cassieText = _config.FakeoutRespawnAnnouncementsUIUAliveSCPSCassieText;
                                        if (scpCount == 1)
                                        {
                                            cassieMessage = cassieMessage.Replace("{scpnum}", $"{scpCount} scpsubject");
                                            cassieText = cassieText.Replace("{scpnum}", $"{scpCount} SCP subject");
                                        }
                                        else
                                        {
                                            cassieMessage = cassieMessage.Replace("{scpnum}", $"{scpCount} scpsubject");
                                            cassieText = cassieText.Replace("{scpnum}", $"{scpCount} SCP subject");
                                            cassieMessage = cassieMessage.Replace("scpsubject", "scpsubjects");
                                            cassieText = cassieText.Replace("SCP subject", "SCP subjects");
                                        }

                                        int randomNatoLetter = random.Next(minValue: 1, maxValue: 26);
                                        int randomNatoNumber = random.Next(minValue: 2, maxValue: 20);
                                        cassieMessage = cassieMessage.Replace("{designation}", $"nato_{GetNatoLetter(randomNatoLetter)} {randomNatoNumber}");
                                        cassieText = cassieText.Replace("{designation}", GetNatoName(randomNatoLetter) + " " + random.Next(randomNatoNumber));
                                        Cassie.MessageTranslated(cassieMessage, cassieText);
                                    }
                                }
                                else if (_config.FakeoutRespawnAnnouncementsMTFFallback)
                                    MtfFakeoutCassie(cassieMessage, cassieText, scpCount);
                                
                                break;
                            
                            case 3:
                                if (_config.FakeoutRespawnAnnouncementsChaosAllow)
                                    Cassie.MessageTranslated(_config.FakeoutRespawnAnnouncementsChaosCassie, _config.FakeoutRespawnAnnouncementsChaosCassieText);
                                else if (_config.FakeoutRespawnAnnouncementsMTFFallback)
                                    MtfFakeoutCassie(cassieMessage, cassieText, scpCount);
                                
                                break;
                            
                            case 4:
                                if(_config.FakeoutRespawnAnnouncementsSerpentsAllow)
                                    Cassie.MessageTranslated(_config.FakeoutRespawnAnnouncementsSerpentsCassie, _config.FakeoutRespawnAnnouncementsSerpentsCassieText);
                                else if (_config.FakeoutRespawnAnnouncementsMTFFallback)
                                    MtfFakeoutCassie(cassieMessage, cassieText, scpCount);
                                
                                break;
                        }
                    }
                    
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Fakeout Respawn Announcements disabled");
                    }

                    break;
                // Rapid Fire Teslas
                case 19:
                    if (_config.RapidFireTelsaEvent)
                    {
                        Log.Debug("Rapid Fire Teslas event is active, running code");
                        _ceRapidFireTeslas = true;
                        _rapidFireTeslas = Timing.RunCoroutine(RapidFireTelsa());
                    }
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Rapid Fire Telsa Gates event is disabled");
                    }
                    
                    break;
                //Players shit themselves
                case 20:
                    if (_config.PlayerShittingPantsEvent)
                    {
                        Log.Debug("Player shitting pants is active, running code");
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            Log.Debug($"Spawning a tantrum where {player} is at");
                            TantrumHazard.PlaceTantrum(player.Position);
                            player.Broadcast(new Exiled.API.Features.Broadcast(_config.PlayerShittingPantsBroadcast,
                                (ushort)_config.BroadcastDisplayTime));
                        }
                    }
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Player shitting pants event is disabled");
                    }

                    break;
                //Router kicking simulator
                case 21:
                    if (_config.RouterKickingSimulatorEvent)
                    {
                        Log.Debug("Router kicking simulator event is active, running code");
                        _ceRouterKickingSimulator = true;
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            Log.Debug($"Showing Router Kicking Simulator start message to {player}");
                            player.Broadcast(new Exiled.API.Features.Broadcast(
                                _config.RouterKickingSimulatorStartBroadcast,
                                (ushort)_config.BroadcastDisplayTime));
                        }
                        
                        if (_config.RouterKickingSimulatorDisablesElevators)
                        {
                            Log.Debug("Locking elevator doors");
                            foreach (Door door in Door.List)
                                if (door is ElevatorDoor { IsLocked: false } elevatorDoor)
                                    elevatorDoor.ChangeLock(DoorLockType.AdminCommand);
                        }

                        if (_config.RouterKickingSimulatorDisablesDecomAndNuke)
                        {
                            Log.Debug("Disabling Decontamination and Nuke");
                            if (!Warhead.IsDetonated)
                            {
                                if (Warhead.IsInProgress)
                                {
                                    Warhead.IsLocked = false;
                                    _previousWarheadTime = Warhead.RealDetonationTimer;
                                    Warhead.DetonationTimer = 90f;
                                    Warhead.Stop();
                                }

                                if (Warhead.Status != WarheadStatus.NotArmed)
                                    Warhead.Status = WarheadStatus.NotArmed;

                                Warhead.IsLocked = true;
                            }

                            if (!Map.IsLczDecontaminated)
                            {
                                _previousDecomTime = DecontaminationController.Singleton.RoundStartTime;
                                DecontaminationController.Singleton.RoundStartTime = -1.0;
                            }
                        }
                        
                        _routerKickingSimulator = Timing.RunCoroutine(RouterKickingSimulator());
                    }
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Router kicking simulator event is disabled");
                    }
                    break;
                //Super Speed Event
                case 22:
                    if (_config.SuperSpeedEvent)
                    {
                        Log.Debug("Super speed event is active, running code");
                        foreach (PlayerAPI player in PlayerAPI.List)
                        {
                            Log.Debug($"Showing Super Speed message to {player}");
                            player.Broadcast(new Exiled.API.Features.Broadcast(_config.SuperSpeedBroadcast, (ushort)_config.BroadcastDisplayTime));
                            Log.Debug($"Giving super speed to {player}, with an intensity of {_config.SuperSpeedIntensity} & a duration of {_config.SuperSpeedDuration}");
                            player.EnableEffect(EffectType.MovementBoost, _config.SuperSpeedIntensity, _config.SuperSpeedDuration, true);
                        }
                    }
                    else
                    {
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        Log.Debug("Super speed event is disabled");
                    }
                    break;
            }
            Log.Debug($"The next event will run in {chaoticEventCycle} seconds");
            yield return Timing.WaitForSeconds(chaoticEventCycle);
        }
    }

    private static IEnumerator<float> WarheadTiming()
    {
        for (;;)
        {
            if (Warhead.DetonationTimer > _config.FakeAutoNukeTimeOut)
            {
                Log.Debug(
                    $"Warhead time hasn't reached {_config.FakeAutoNukeTimeOut} yet. Waiting half a second and checking again");
                Log.Debug($"Detonation Time is {Warhead.DetonationTimer}");
            }
            else
            {
                Log.Debug("Time has been reached, stopping warhead");
                Warhead.IsLocked = false;
                if (_config.FakeAutoNukeRestoresOldTime)
                    Warhead.DetonationTimer = _previousWarheadTime;
                Warhead.Stop();
                foreach (PlayerAPI player in PlayerAPI.List)
                {
                    Log.Debug($"Displaying the fake out message to {player}");
                    player.Broadcast(new Exiled.API.Features.Broadcast(_config.FakeAutoNukeFakeoutText,
                        (ushort)_config.BroadcastDisplayTime));
                }

                _ceFakeWarheadEvent = false;
                yield break;
            }
            yield return Timing.WaitForSeconds(0.5f);
        }
    }

    private static IEnumerator<float> RapidFireTelsa(/*TriggeringTeslaEventArgs ev*/)
    {
        float roundTimeFromEventStart = (float)Round.ElapsedTime.TotalSeconds;
        float regularActivationTime = 0;
        float regularIdleRange = 0;
        float regularTriggerRange = 0;
        float regularCooldownTime = 0;
        float modifiedActivationTime = 0;
        float modifiedIdleRange = 0;
        float modifiedTriggerRange = 0;
        float modifiedCooldownTime = 0;
        
        foreach (Tesla teslaGate in Tesla.List)
        {
            regularActivationTime = teslaGate.ActivationTime;
            regularCooldownTime = teslaGate.CooldownTime;
            regularTriggerRange = teslaGate.TriggerRange;
            regularIdleRange = teslaGate.IdleRange;
            
            Log.Debug($"{teslaGate.ActivationTime}, {teslaGate.TriggerRange}, {teslaGate.CooldownTime}, {teslaGate.IdleRange}");
            
            teslaGate.ActivationTime = _config.RapidFireTeslaEventActivationTime;
            teslaGate.IdleRange = _config.RapidFireTeslaEventIdleRange;
            teslaGate.TriggerRange = _config.RapidFireTeslaEventTriggerRange;
            teslaGate.CooldownTime = _config.RapidFireTeslaEventCooldownTime;
            
            if (Plugin.Instance.Config.Debug)
            {
                modifiedActivationTime = teslaGate.ActivationTime;
                modifiedIdleRange = teslaGate.IdleRange;
                modifiedTriggerRange = teslaGate.TriggerRange;
                modifiedCooldownTime = teslaGate.CooldownTime;
                Log.Debug("Going from top down, modified activation time, idle range, trigger range, cooldown time");
                Log.Debug(modifiedActivationTime);
                Log.Debug(modifiedIdleRange);
                Log.Debug(modifiedTriggerRange);
                Log.Debug(modifiedCooldownTime);
            }
        }
            
        for (;;)
        {
            if (Round.ElapsedTime.TotalSeconds < roundTimeFromEventStart + _config.RapidFireTeslaEventTiming)
            {
                Log.Debug(
                    "Round time hasn't reached the threshold to end rapid fire teslas event, waiting half a second and checking again");
                Log.Debug($"Current Round Time is: {Round.ElapsedTime.TotalSeconds}. Threshold is: {roundTimeFromEventStart + _config.RapidFireTeslaEventTiming}");
                Log.Debug(
                    $"Act {modifiedActivationTime}, Idl {modifiedIdleRange}, Tri {modifiedTriggerRange}, Coo {modifiedCooldownTime}");
            }
            else
            {
                Log.Debug("Time threshold has been reached, ending event");
                _ceRapidFireTeslas = false;
                foreach (Tesla teslaGate in Tesla.List)
                {
                    teslaGate.ActivationTime = regularActivationTime;
                    teslaGate.IdleRange = regularIdleRange;
                    teslaGate.TriggerRange = regularTriggerRange;
                    teslaGate.CooldownTime = regularCooldownTime;
                }
                yield break;
            }
            yield return Timing.WaitForSeconds(0.5f);
        }
    }

    private static IEnumerator<float> RouterKickingSimulator()
    {
        int routerKickCount = 0;
        Dictionary<int, Vector3> playerPositions = new Dictionary<int, Vector3>();
        Log.Debug($"Waiting for {_config.RouterKickingSimulatorStartWaitTime} before starting properly");
        Timing.WaitForSeconds(_config.RouterKickingSimulatorStartWaitTime);
        for (;;)
        {
            if (routerKickCount != _config.RouterKickingSimulatorRouterKickAmount)
            {
                foreach (PlayerAPI player in PlayerAPI.List)
                    if (player.Role.Team != Team.Dead)
                    {
                        Log.Debug($"{player} is not dead, logging position");
                        playerPositions[player.Id] = player.Position;
                    }

                Log.Debug($"Waiting {_config.RouterKickingSimulatorLagTime} seconds");
                Timing.WaitForSeconds(_config.RouterKickingSimulatorLagTime);
                
                foreach (PlayerAPI player in PlayerAPI.List)
                {
                    if (playerPositions.TryGetValue(player.Id, out Vector3 position))
                    {
                        Log.Debug($"Getting {player} previously logged position");
                        player.Position = position;
                    }
                    else
                        Log.Debug($"{player} isn't in the list. Most likely due to not being connected or alive during the last cycle.");
                }
                routerKickCount++;
                Log.Debug($"Adding to the router kick count, which is now {routerKickCount}");
            }
            else
            {
                Log.Debug("Amount of router kicks has been reached, ending event");
                foreach (PlayerAPI player in PlayerAPI.List)
                {
                    Log.Debug($"Showing Router Kicking Simulator end message to {player}");
                    player.Broadcast(new Exiled.API.Features.Broadcast(
                        _config.RouterKickingSimulatorEndBroadcast,
                        (ushort)_config.BroadcastDisplayTime));
                }
                
                if (_config.RouterKickingSimulatorDisablesElevators)
                {
                    Log.Debug("Unlocking elevator doors");
                    foreach (Door door in Door.List)
                        if (door is ElevatorDoor { IsLocked: true } elevatorDoor)
                            elevatorDoor.ChangeLock(DoorLockType.AdminCommand);
                }

                if (_config.RouterKickingSimulatorDisablesDecomAndNuke)
                {
                    Log.Debug("Waiting a fixed 15 seconds before reenabling decom & nuke");
                    Timing.WaitForSeconds(15);
                    Log.Debug("15 seconds has passed, reenabling decom and nuke");
                    if (!Map.IsLczDecontaminated)
                    {
                        DecontaminationController.Singleton.RoundStartTime = _previousDecomTime;
                    }

                    if (!Warhead.IsDetonated)
                    {
                        Warhead.IsLocked = false;
                        Warhead.Status = WarheadStatus.Armed;
                        Warhead.DetonationTimer = _previousWarheadTime;
                    }
                }

                _ceRouterKickingSimulator = false;
                
                yield break;
            }
            Log.Debug($"Waiting {_config.RouterKickingSimulatorTimeBetweenRouterKicks} seconds");
            yield return Timing.WaitForSeconds(_config.RouterKickingSimulatorTimeBetweenRouterKicks);
        }
    }
    
    private static readonly ItemType[] WeaponTypes =
    [
        ItemType.GunA7,
        ItemType.GunCom45,
        ItemType.GunCrossvec,
        ItemType.GunLogicer,
        ItemType.GunRevolver,
        ItemType.GunShotgun,
        ItemType.GunAK,
        ItemType.GunCOM15,
        ItemType.GunCOM18,
        ItemType.GunE11SR,
        ItemType.GunFSP9,
        ItemType.GunFRMG0,
        ItemType.Jailbird,
        ItemType.ParticleDisruptor,
        ItemType.GrenadeFlash,
        ItemType.GrenadeHE
    ];
    private static bool IsWeapon(Item item)
    {
        return WeaponTypes.Contains(item.Type);
    }
    private static PlayerAPI GetRandomPlayerFBI()
    {
        Log.Debug("Getting a random player for the FBI Open Up Chaos Event");
        Random random = new Random();
        List<PlayerAPI> fbiOpenUpPossibleTargets = PlayerAPI.List.Where(p => p.Role != (RoleTypeId)Team.FoundationForces || p.Role != (RoleTypeId)Team.Scientists || p.Role != (RoleTypeId)Team.Dead).ToList();
        if (fbiOpenUpPossibleTargets.Count == 0)
            return null;
        int index = random.Next(fbiOpenUpPossibleTargets.Count);
        return fbiOpenUpPossibleTargets[index];
    }
    private static string GetNatoName(int randomUnit)
    {
        Log.Debug("Getting a Nato Name (even though its not called the Nato alphabet, its called the phonetic alphabet");
        Dictionary<int, string> natoAlphabet = new Dictionary<int, string>()
        {
            {1, "ALPHA"},
            {2, "BRAVO"},
            {3, "CHARLIE"},
            {4, "DELTA"},
            {5, "ECHO"},
            {6, "FOXTROT"},
            {7, "GOLF"},
            {8, "HOTEL"},
            {9, "INDIA"},
            {10, "JULIET"},
            {11, "KILO"},
            {12, "LIMA"},
            {13, "MIKE"},
            {14, "NOVEMBER"},
            {15, "OSCAR"},
            {16, "PAPA"},
            {17, "QUEBEC"},
            {18, "ROMEO"},
            {19, "SIERRA"},
            {20, "TANGO"},
            {21, "UNIFORM"},
            {22, "VICTOR"},
            {23, "WHISKEY"},
            {24, "XRAY"},
            {25, "YANKEE"},
            {26, "ZULU" },
        };
        return natoAlphabet[randomUnit];
    }

    private static string GetNatoLetter(int randomUnit)
    {
        Log.Debug("Getting a random letter");
        Dictionary<int, string> natoLetter = new Dictionary<int, string>()
        {
            { 1, "A" },
            { 2, "B" },
            { 3, "C" },
            { 4, "D" },
            { 5, "E" },
            { 6, "F" },
            { 7, "G" },
            { 8, "H" },
            { 9, "I" },
            { 10, "J" },
            { 11, "K" },
            { 12, "L" },
            { 13, "M" },
            { 14, "N" },
            { 15, "O" },
            { 16, "P" },
            { 17, "Q" },
            { 18, "R" },
            { 19, "S" },
            { 20, "T" },
            { 21, "U" },
            { 22, "V" },
            { 23, "W" },
            { 24, "X" },
            { 25, "Y" },
            { 26, "Z" },
        };
        return natoLetter[randomUnit];
    }

    private static void MtfFakeoutCassie(string cassieMessage, string cassieText, int scpCount)
    {
        Log.Debug("Running Mtf Fakeout Cassie handle");
        Random random = new Random();
        foreach (Player player in Player.List)
        {
            {
                if (player.Role.Team == Team.SCPs)
                    scpCount++;
            }

            if (scpCount == 0)
            {
                cassieMessage = _config.FakeoutRespawnAnnouncementsMTFSCPSDeadCassie;
                cassieText = _config.FakeoutRespawnAnnouncementsMTFSCPSDeadCassieText;
                int randomNatoLetter = random.Next(minValue: 1, maxValue: 26);
                int randomNatoNumber = random.Next(minValue: 2, maxValue: 20);
                cassieMessage = cassieMessage.Replace("{designation}",
                    $"nato_{GetNatoLetter(randomNatoLetter)} {randomNatoNumber}");
                cassieText = cassieText.Replace("{designation}",
                    GetNatoName(randomNatoLetter) + " " + random.Next(randomNatoNumber));
                Cassie.MessageTranslated(cassieMessage, cassieText);
            }

            else
            {
                cassieMessage = _config.FakeoutRespawnAnnouncementsMTFAliveSCPSCassie;
                cassieText = _config.FakeoutRespawnAnnouncementsMTFAliveSCPSCassieText;
                if (scpCount == 1)
                {
                    cassieMessage = cassieMessage.Replace("{scpnum}", $"{scpCount} scpsubject");
                    cassieText = cassieText.Replace("{scpnum}", $"{scpCount} SCP subject");
                }

                else
                {
                    cassieMessage = cassieMessage.Replace("{scpnum}", $"{scpCount} scpsubject");
                    cassieText = cassieText.Replace("{scpnum}", $"{scpCount} SCP subject");
                    cassieMessage = cassieMessage.Replace("scpsubject", "scpsubjects");
                    cassieText = cassieText.Replace("SCP subject", "SCP subjects");
                }

                int randomNatoLetter = random.Next(minValue: 1, maxValue: 26);
                int randomNatoNumber = random.Next(minValue: 2, maxValue: 20);
                cassieMessage = cassieMessage.Replace("{designation}",
                    $"nato_{GetNatoLetter(randomNatoLetter)} {randomNatoNumber}");
                cassieText = cassieText.Replace("{designation}",
                    GetNatoName(randomNatoLetter) + " " + random.Next(randomNatoNumber));
                Cassie.MessageTranslated(cassieMessage, cassieText);
            }
        }
    }

    public static void EndEvent()
    {
        if (!_ceStarted) return;
        _ceStarted = false;
        Log.Debug("Killing main Coroutine Handle for Chaotic Events");
        Timing.KillCoroutines(_choaticHandle);
        Plugin.ActiveEvent -= 1;
        if (_ceMedicalItemEvent)
        {
            Log.Debug("Removing On Using Item Completed Event");
            PlayerEvent.UsingItemCompleted -= Plugin.Instance.ServerEventsMainEventHandler.OnUsingMedicalItemCE;
            _ceMedicalItemEvent = false;
        }
        
        if (_ceFakeWarheadEvent)
        {
            Log.Debug("Killing fake warhead coroutine");
            Timing.KillCoroutines(_fakeWarheadHandle);
            _ceFakeWarheadEvent = false;
        }

        if (_ceRapidFireTeslas)
        {
            Log.Debug("Killing rapid fire teslas coroutine");
            Timing.KillCoroutines(_rapidFireTeslas);
            _ceRapidFireTeslas = false;
        }

        if (_ceRouterKickingSimulator)
        {
            Log.Debug("Killing router kicking coroutine");
            Timing.KillCoroutines(_routerKickingSimulator);
            _ceRouterKickingSimulator = false;
        }
    }
}