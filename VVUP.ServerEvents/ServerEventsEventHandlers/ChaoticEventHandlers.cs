using System;
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
using UnityEngine;
using VVUP.ServerEvents.ServerEventsConfigs;
using Cassie = Exiled.API.Features.Cassie;
using Item = Exiled.API.Features.Items.Item;
using Map = Exiled.API.Features.Map;
using PlayerAPI = Exiled.API.Features.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Tesla = Exiled.API.Features.TeslaGate;
using Warhead = Exiled.API.Features.Warhead;

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
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
        private static bool _realSimonSaysEvent;
        private static bool _fakeSimonSaysEvent;
        private static float _previousWarheadTime;
        private static double _previousDecomTime;

        public ChaoticEventHandlers()
        {
            Log.Debug("VVUP Server Events, Chaotic: Checking if Chaotic Event has already started");
            if (_ceStarted) return;
            _config = Plugin.Instance.Config.ChaoticConfig;
            Plugin.ActiveEvent += 1;
            _ceStarted = true;
            LabApi.Features.Wrappers.Cassie.Message(_config.StartEventCassieMessage, _config.StartEventCassieText);
            _choaticHandle = Timing.RunCoroutine(ChaoticTiming());
        }

        private static IEnumerator<float> ChaoticTiming()
        {
            if (!_ceStarted)
                yield break;
            for (;;)
            {
                float chaoticEventCycle = _config.TimeForChaosEvent;
                int chaosRandomNumber = Base.GetRandomNumber.GetRandomInt(1, 27);
                Log.Debug(chaosRandomNumber);
                if (_config.ChaosEventEndsOtherEvents)
                {
                    Log.Debug("VVUP Server Events, Chaotic: Event ends other events active, ending other events.");
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
                            Log.Debug("VVUP Server Events, Chaotic: Item Steal Chaos Event is active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug("VVUP Server Events, Chaotic: Checking if players aren't a SCP or is dead");
                                if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: Showing Broadcast to {player.Nickname} saying their items got stolen");
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.ItemStolenText);
                                    Log.Debug($"VVUP Server Events, Chaotic: Clearing Items from {player.Nickname}");
                                    player.ClearItems();
                                }
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Item Steal Chaos Event disabled");
                        }

                        break;
                    // Give Random Item
                    case 2:
                        if (_config.GiveRandomItemEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Give Random Item Event is active, running code");
                            Log.Debug("VVUP Server Events, Chaotic: Getting a list of both standard items and custom items");
                            ItemType[] standardItems = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray();
                            List<CustomItem> customItems = CustomItem.Registered.ToList();
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug("VVUP Server Events, Chaotic: Checking if players aren't a SCP or is dead");
                                if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                                {
                                    int randomItemGiveRng = Base.GetRandomNumber.GetRandomInt(0, 2);
                                    Log.Debug($"VVUP Server Events, Chaotic: Deciding if {player.Nickname} gets a Custom Item or a Regular Item");
                                    if (_config.GiveRandomItemCustomitems)
                                    {
                                        switch (randomItemGiveRng)
                                        {
                                            case 1:
                                                CustomItem randomCustomItem =
                                                    customItems[Base.GetRandomNumber.GetRandomInt(customItems.Count)];
                                                randomCustomItem.Give(player);
                                                Log.Debug($"VVUP Server Events, Chaotic: {player.Nickname} has received custom item id {randomCustomItem}");
                                                break;
                                            case 2:
                                                ItemType randomStandardItem =
                                                    standardItems[Base.GetRandomNumber.GetRandomInt(standardItems.Length)];
                                                player.AddItem(randomStandardItem);
                                                Log.Debug($"VVUP Server Events, Chaotic: {player.Nickname} has received item id {randomStandardItem}");
                                                break;
                                        }
                                    }

                                    else
                                    {
                                        ItemType randomStandardItem =
                                            standardItems[Base.GetRandomNumber.GetRandomInt(standardItems.Length)];
                                        player.AddItem(randomStandardItem);
                                    }

                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.GiveRandomItemText);
                                }
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Give Random Item Event is disabled.");
                        }

                        break;
                    // Random Teleport
                    case 3:
                        if (_config.RandomTeleportEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Random Teleport Event is active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (!_config.TeleportToFacilityAfterNuke && Warhead.IsDetonated)
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: Warhead has been detonated, teleporting {player.Nickname} somewhere on Surface");
                                    player.Teleport(Room.List.Where(r => r.Type is RoomType.Surface).GetRandomValue());
                                }

                                else if (!_config.TeleportToLightAfterDecom &&
                                         Map.DecontaminationState == DecontaminationState.Finish)
                                {
                                    Log.Debug(
                                        $"VVUP Server Events, Chaotic: Light has been decontaminated, teleporting {player.Nickname} somewhere else in the facility");
                                    player.Teleport(Room.List.Where(r => r.Zone is not ZoneType.LightContainment)
                                        .GetRandomValue());
                                }

                                else
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: Teleporting {player.Nickname} randomly into the facility");
                                    player.Teleport(Room.List.GetRandomValue());
                                }

                                player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.RandomTeleportText);
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Random Teleport Event is disabled");
                        }

                        break;
                    // Fake Auto Nuke
                    case 4:
                        if (_config.FakeAutoNuke)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Fake auto nuke event is active, running code");
                            if (_ceFakeWarheadEvent)
                                Log.Debug("VVUP Server Events, Chaotic: Fake Warhead Event is already running");
                            else if (!Warhead.IsDetonated || !Warhead.IsInProgress)
                            {
                                _ceFakeWarheadEvent = true;
                                Log.Debug(
                                    "VVUP Server Events, Chaotic: Saving previous warhead time in the event of the nuke being activated before the fake auto nuke");
                                if (_config.FakeAutoNukeRestoresOldTime)
                                    _previousWarheadTime = Warhead.RealDetonationTimer;
                                Log.Debug("VVUP Server Events, Chaotic: Starting warhead");
                                Warhead.Start();
                                Log.Debug("VVUP Server Events, Chaotic: Checking if the Warhead is locked, if not lock it");
                                if (!Warhead.IsLocked)
                                    Warhead.IsLocked = true;
                                foreach (PlayerAPI player in PlayerAPI.List)
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: Displaying the fake autonuke start message to {player.Nickname}");
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime,
                                        _config.FakeAutoNukeStartText);
                                }

                                Log.Debug("VVUP Server Events, Chaotic: Starting a Coroutine to track warhead timing");
                                _fakeWarheadHandle = Timing.RunCoroutine(WarheadTiming());
                            }
                            else
                                Log.Debug("VVUP Server Events, Chaotic: Warhead is either detonated or is in progress, not running this event");
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Fake auto nuke event is disabled");
                        }

                        break;
                    // Remove Weapons
                    case 5:
                        if (_config.RemoveWeaponsEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Remove Weapons event is active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                                {
                                    foreach (var item in player.Items)
                                    {
                                        Log.Debug($"VVUP Server Events, Chaotic: Checking if {player.Nickname} has a weapon");
                                        if (IsWeapon(item))
                                        {
                                            Log.Debug($"VVUP Server Events, Chaotic: Taking away {player.Nickname}'s {item}");
                                            player.RemoveItem(item);
                                            player.Broadcast((ushort)_config.BroadcastDisplayTime, 
                                                _config.RemoveWeaponsText);
                                        }
                                    }
                                }
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Remove Weapons Event is disabled");
                        }

                        break;
                    // Give Weapons
                    case 6:
                        if (_config.GiveRandomWeaponsEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Giving random weapons is active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: Checking if {player.Nickname} is able to get a random weapon");
                                    ItemType randomWeapon = WeaponTypes[Base.GetRandomNumber.GetRandomInt(WeaponTypes.Count)];
                                    if (_config.GiveRandomWeaponsToUnarmedPlayers)
                                    {
                                        foreach (var item in player.Items)
                                        {
                                            Log.Debug($"VVUP Server Events, Chaotic: Checking if {player.Nickname} has a weapon");
                                            if (IsWeapon(item))
                                            {
                                                Log.Debug($"VVUP Server Events, Chaotic: {player.Nickname} has a weapon, not granting another");
                                                player.Broadcast((ushort)_config.BroadcastDisplayTime,
                                                    _config.GiveRandomWeaponsTextFail);
                                            }

                                            else
                                            {
                                                if (_config.GiveAllRandomWeapons)
                                                {
                                                    Log.Debug($"VVUP Server Events, Chaotic: Giving {player.Nickname} a random weapon");
                                                    player.AddItem(randomWeapon);
                                                }

                                                else if (_config.GiveRandomWeaponsDefined == null)
                                                {
                                                    Log.Warn(
                                                        "VVUP Server Events, Chaotic: GiveAllRandomWeapons is false but the GiveRandomWeaponsDefined is empty! Falling back to any random weapon.");
                                                    player.AddItem(randomWeapon);
                                                }

                                                else
                                                {
                                                    Log.Debug($"VVUP Server Events, Chaotic: Giving {player.Nickname} a predefined weapon");
                                                    player.AddItem(
                                                        _config.GiveRandomWeaponsDefined[
                                                            Base.GetRandomNumber.GetRandomInt(_config.GiveRandomWeaponsDefined.Count)]);
                                                }

                                                player.Broadcast((ushort)_config.BroadcastDisplayTime,
                                                    _config.GiveRandomWeaponsText);
                                            }
                                        }
                                    }

                                    else
                                    {
                                        if (player.IsInventoryFull)
                                        {
                                            Log.Debug($"VVUP Server Events, Chaotic: {player.Nickname}'s inventory is full, not giving any weapon");
                                            player.Broadcast((ushort)_config.BroadcastDisplayTime,
                                                _config.GiveRandomWeaponsTextFail);
                                        }

                                        else
                                        {
                                            if (_config.GiveAllRandomWeapons)
                                            {
                                                Log.Debug($"VVUP Server Events, Chaotic: Giving {player.Nickname} a random weapon");
                                                player.AddItem(randomWeapon);
                                            }

                                            else if (_config.GiveRandomWeaponsDefined == null)
                                            {
                                                Log.Warn(
                                                    "VVUP Server Events, Chaotic: GiveAllRandomWeapons is false but the GiveRandomWeaponsDefined is empty! Falling back to any random weapon.");
                                                player.AddItem(randomWeapon);
                                            }

                                            else
                                            {
                                                Log.Debug($"VVUP Server Events, Chaotic: Giving {player.Nickname} a predefined weapon");
                                                player.AddItem(
                                                    _config.GiveRandomWeaponsDefined[
                                                        Base.GetRandomNumber.GetRandomInt(_config.GiveRandomWeaponsDefined.Count)]);
                                            }

                                            player.Broadcast((ushort)_config.BroadcastDisplayTime,
                                                _config.GiveRandomWeaponsText);
                                        }
                                    }
                                }
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Giving random weapons event is disabled");
                        }

                        break;
                    // Death Match
                    case 7:
                        if (_config.DeathMatchEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Death Match Event is active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug("VVUP Server Events, Chaotic: Checking if SCPs should be affected");
                                if (!_config.DeathMatchEventAffectsSCPs)
                                {
                                    Log.Debug("VVUP Server Events, Chaotic: SCPs aren't affected");
                                    if (player.Role.Team != Team.SCPs || player.Role.Team != Team.Dead)
                                    {
                                        Log.Debug($"VVUP Server Events, Chaotic: Setting {player.Nickname}'s health to {_config.DeathMatchHealth}");
                                        player.Health = _config.DeathMatchHealth;
                                        player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.DeathMatchText);
                                    }
                                }

                                else
                                {
                                    Log.Debug("VVUP Server Events, Chaotic: SCPs are affected");
                                    Log.Debug($"VVUP Server Events, Chaotic: Setting {player.Nickname}'s health to {_config.DeathMatchHealth}");
                                    player.Health = _config.DeathMatchHealth;
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.DeathMatchText);
                                }
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Death Match Event is disabled");
                        }

                        break;
                    // Blackout
                    case 8:
                        if (_config.ChaosEventEnablesOtherEvents)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event Enables other Events true, running Blackout Event");
                            var blackoutEventHandlers = new BlackoutEventHandlers();
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event enabling other events is disabled");
                        }

                        break;
                    // Freezing Temperatures
                    case 9:
                        if (_config.ChaosEventEnablesOtherEvents)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event Enables other Events true, running Freezing Temperature Event");
                            var freezingTemperaturesEventHandlers = new FreezingTemperaturesEventHandlers();
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event enabling other events is disabled");
                        }

                        break;
                    // Peanut Hydra
                    case 10:
                        if (_config.ChaosEventEnablesOtherEvents)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event Enables other Events true, running Peanut Hydra Event");
                            var peanutHydraEventHandlers = new PeanutHydraEventHandlers();
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event enabling other events is disabled");
                        }

                        break;
                    // Peanut Infection
                    case 11:
                        if (_config.ChaosEventEnablesOtherEvents)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event Enables other Events true, running Peanut Infection Event");
                            var peanutInfectionEventHandlers = new PeanutInfectionEventHandlers();
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event enabling other events is disabled");
                        }

                        break;
                    // Short People Event
                    case 12:
                        if (_config.ChaosEventEnablesOtherEvents)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event Enables other Events true, running Short People Event");
                            var shortEventHandlers = new ShortEventHandlers();
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event enabling other events is disabled");
                        }

                        break;
                    // Variable Lights
                    case 13:
                        if (_config.ChaosEventEnablesOtherEvents)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event Enables other Events true, running Variable Lights Event");
                            var variableLightsEvent = new VariableLightsEventHandlers();
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event enabling other events is disabled");
                        }

                        break;
                    // FBI Open Up Event
                    case 14:
                        if (_config.FBIOpenUpEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: FBI Open Up Event active, running code");
                            Log.Debug("VVUP Server Events, Chaotic: Getting a random non-foundation player");
                            PlayerAPI FBIOpenUpTarget = GetRandomPlayerFBI();
                            Log.Debug(
                                $"VVUP Server Events, Chaotic: Showing {FBIOpenUpTarget} the warning message they are about to have the foundation teleport to them");
                            FBIOpenUpTarget.Broadcast((ushort)_config.BroadcastDisplayTime, _config.FBIOpenUpTargetText);
                            yield return Timing.WaitForSeconds(_config.FBITeleportTime);
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (player.Role.Team == Team.FoundationForces)
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: {player.Nickname} is on the MTF, Showing warning text");
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.FBIOpenUpMTFText);
                                    Log.Debug($"VVUP Server Events, Chaotic: Teleporting {player.Nickname} to {FBIOpenUpTarget}");
                                    player.Teleport(FBIOpenUpTarget.Position);
                                }
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: FBI Open Up Event disabled");
                        }

                        break;
                    // Grenade Feet
                    case 15:
                        if (_config.GrenadeFeetEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Grenade Feet Event active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug($"VVUP Server Events, Chaotic: Grenade feet warning being shown to {player.Nickname}");
                                player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.GrenadeFeetText);
                            }

                            yield return Timing.WaitForSeconds(Base.GetRandomNumber.GetRandomFloat(1, 50));

                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug($"VVUP Server Events, Chaotic: Spawning a grenade on {player.Nickname}");
                                ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                                if (_config.GrenadeFeetRandomFuse)
                                    grenade.FuseTime = Base.GetRandomNumber.GetRandomFloat(1, 50);
                                else
                                    grenade.FuseTime = _config.GrenadeFeetFuse;
                                grenade.SpawnActive(player.Position);
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Grenade Feet Event disabled");
                        }

                        break;
                    // Unsafe Medical Items
                    case 16:
                        if (_config.UnsafeMedicalItemsEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Unsafe medical items event active, running code");
                            Log.Debug("VVUP Server Events, Chaotic: Activating Event Handlers for on using medical item");
                            PlayerEvent.UsingItemCompleted +=
                                Plugin.Instance.ServerEventsMainEventHandler.OnUsingMedicalItemCE;
                            _ceMedicalItemEvent = true;
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (player.Role.Team != Team.SCPs)
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: Displaying Unsafe Medical Items Warning to {player.Nickname}");
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.UnsafeMedicalItemsText);
                                }
                            }

                            if (_config.UnsafeMedicalItemsUseRandomTime)
                                yield return Timing.WaitForSeconds(Base.GetRandomNumber.GetRandomFloat(1, 50));
                            else
                                yield return Timing.WaitForSeconds(_config.UnsafeMedicalItemsFixedTime);
                            Log.Debug("VVUP Server Events, Chaotic: Disabling Event Handlers for on using medical item events");
                            PlayerEvent.UsingItemCompleted -=
                                Plugin.Instance.ServerEventsMainEventHandler.OnUsingMedicalItemCE;
                            _ceMedicalItemEvent = false;
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (player.Role.Team != Team.SCPs)
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: Displaying Unsafe Medical Items Warning to {player.Nickname}");
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.UnsafeMedicalItemsSafeToUseText);
                                }
                            }
                        }
                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Unsafe medical Items Event disabled");
                        }

                        break;
                    // Name Redacted
                    case 17:
                        if (_config.ChaosEventEnablesOtherEvents)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event Enables other Events true, running Name Redacted Event");
                            var nameRedactedHandlers = new NameRedactedEventHandlers();
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Chaos Event enabling other events is disabled");
                        }

                        break;
                    // Fakeout Respawn Announcements
                    case 18:
                        if (_config.FakeoutRespawnAnnouncementsEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Fakeout Respawn Announcements active, running code");
                            int fakeoutRespawnRandom = Base.GetRandomNumber.GetRandomInt(1, 5);
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
                                        int randomNatoLetter = Base.GetRandomNumber.GetRandomInt(1, 27);
                                        int randomNatoNumber = Base.GetRandomNumber.GetRandomInt(2, 21);
                                        if (scpCount == 0)
                                        {
                                            cassieMessage = _config.FakeoutRespawnAnnouncementsUIUSCPSDeadCassie;
                                            cassieText = _config.FakeoutRespawnAnnouncementsUIUSCPSDeadCassieText;
                                            cassieMessage = cassieMessage.Replace("{designation}",
                                                $"nato_{GetNatoLetter(randomNatoLetter)} {randomNatoNumber}");
                                            cassieText = cassieText.Replace("{designation}",
                                                GetNatoName(randomNatoLetter) + " " + randomNatoNumber);
                                        }
                                        else
                                        {
                                            cassieMessage = _config.FakeoutRespawnAnnouncementsUIUAliveSCPSCassie;
                                            cassieText = _config.FakeoutRespawnAnnouncementsUIUAliveSCPSCassieText;
                                            if (scpCount == 1)
                                            {
                                                cassieMessage = cassieMessage.Replace("{scpnum}",
                                                    $"{scpCount} scpsubject");
                                                cassieText = cassieText.Replace("{scpnum}", $"{scpCount} SCP subject");
                                            }
                                            else
                                            {
                                                cassieMessage = cassieMessage.Replace("{scpnum}",
                                                    $"{scpCount} scpsubject");
                                                cassieText = cassieText.Replace("{scpnum}", $"{scpCount} SCP subject");
                                                cassieMessage = cassieMessage.Replace("scpsubject", "scpsubjects");
                                                cassieText = cassieText.Replace("SCP subject", "SCP subjects");
                                            }
                                            cassieMessage = cassieMessage.Replace("{designation}",
                                                $"nato_{GetNatoLetter(randomNatoLetter)} {randomNatoNumber}");
                                            cassieText = cassieText.Replace("{designation}",
                                                GetNatoName(randomNatoLetter) + " " + randomNatoNumber);
                                        }
                                        LabApi.Features.Wrappers.Cassie.Message(cassieMessage, cassieText);
                                    }
                                    else if (_config.FakeoutRespawnAnnouncementsMTFFallback)
                                        MtfFakeoutCassie(cassieMessage, cassieText, scpCount);

                                    break;

                                case 3:
                                    if (_config.FakeoutRespawnAnnouncementsChaosAllow)
                                        LabApi.Features.Wrappers.Cassie.Message(_config.FakeoutRespawnAnnouncementsChaosCassie,
                                            _config.FakeoutRespawnAnnouncementsChaosCassieText);
                                    else if (_config.FakeoutRespawnAnnouncementsMTFFallback)
                                        MtfFakeoutCassie(cassieMessage, cassieText, scpCount);

                                    break;

                                case 4:
                                    if (_config.FakeoutRespawnAnnouncementsSerpentsAllow)
                                        LabApi.Features.Wrappers.Cassie.Message(_config.FakeoutRespawnAnnouncementsSerpentsCassie,
                                            _config.FakeoutRespawnAnnouncementsSerpentsCassieText);
                                    else if (_config.FakeoutRespawnAnnouncementsMTFFallback)
                                        MtfFakeoutCassie(cassieMessage, cassieText, scpCount);

                                    break;
                                
                                default:
                                    if (_config.FakeoutRespawnAnnouncementsMTFAllow)
                                        MtfFakeoutCassie(cassieMessage, cassieText, scpCount);

                                    break;
                            }
                        }

                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Fakeout Respawn Announcements disabled");
                        }

                        break;
                    // Rapid Fire Teslas
                    case 19:
                        if (_config.RapidFireTelsaEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Rapid Fire Teslas event is active, running code");
                            _ceRapidFireTeslas = true;
                            _rapidFireTeslas = Timing.RunCoroutine(RapidFireTelsa());
                        }
                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Rapid Fire Telsa Gates event is disabled");
                        }

                        break;
                    //Players shit themselves
                    case 20:
                        if (_config.PlayerShittingPantsEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Player shitting pants is active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug($"VVUP Server Events, Chaotic: Spawning a tantrum where {player.Nickname} is at");
                                TantrumHazard.PlaceTantrum(player.Position);
                                player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.PlayerShittingPantsBroadcast);
                            }
                        }
                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Player shitting pants event is disabled");
                        }

                        break;
                    //Router kicking simulator
                    case 21:
                        if (_config.RouterKickingSimulatorEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Router kicking simulator event is active, running code");
                            _ceRouterKickingSimulator = true;
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug($"VVUP Server Events, Chaotic: Showing Router Kicking Simulator start message to {player.Nickname}");
                                player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.RouterKickingSimulatorStartBroadcast);
                            }

                            if (_config.RouterKickingSimulatorDisablesElevators)
                            {
                                Log.Debug("VVUP Server Events, Chaotic: Locking elevator doors");
                                foreach (Door door in Door.List)
                                    if (door is ElevatorDoor { IsLocked: false } elevatorDoor)
                                        elevatorDoor.ChangeLock(DoorLockType.AdminCommand);
                            }

                            if (_config.RouterKickingSimulatorDisablesDecomAndNuke)
                            {
                                Log.Debug("VVUP Server Events, Chaotic: Disabling Decontamination and Nuke");
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
                            Log.Debug("VVUP Server Events, Chaotic: Router kicking simulator event is disabled");
                        }

                        break;
                    //Super Speed Event
                    case 22:
                        if (_config.SuperSpeedEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Super speed event is active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                Log.Debug($"VVUP Server Events, Chaotic: Showing Super Speed message to {player.Nickname}");
                                player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SuperSpeedBroadcast);
                                Log.Debug(
                                    $"VVUP Server Events, Chaotic: Giving super speed to {player.Nickname}, with an intensity of {_config.SuperSpeedIntensity} & a duration of {_config.SuperSpeedDuration}");
                                player.EnableEffect(EffectType.MovementBoost, _config.SuperSpeedIntensity,
                                    _config.SuperSpeedDuration, true);
                            }
                        }
                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Super speed event is disabled");
                        }

                        break;
                    // Drop Items Event
                    case 23:
                        if (_config.DropInventoryEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Drop Inventory event is active, running code");
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (player.IsAlive && player.Role.Team != Team.SCPs && !player.Items.IsEmpty())
                                {
                                    Log.Debug($"VVUP Server Events, Chaotic: Dropping items for {player.Nickname}");
                                    player.DropItems();
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.DropInventoryText);
                                }
                            }
                        }
                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Dropping Inventory event is disabled");
                        }

                        break;
                    // Teleport players to allies event
                    case 24:
                        if (_config.TeleportToAlliesEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Running Teleport to Allies event");
                            List<PlayerAPI> players = PlayerAPI.List.Where(p => p.Role.Team != Team.Dead && p.Role != RoleTypeId.Scp079).ToList();
                            List<PlayerAPI> teleportedPlayers = new List<PlayerAPI>();
                            foreach (PlayerAPI player in players)
                            {
                                if (teleportedPlayers.Contains(player))
                                    continue;
                                List<PlayerAPI> allies = players.Where(p => p.Role.Team == player.Role.Team && !teleportedPlayers.Contains(p) && p != player).ToList();
                                if (allies.Count == 0)
                                    continue;
                                PlayerAPI ally = allies[Base.GetRandomNumber.GetRandomInt(allies.Count)];
                                Log.Debug($"VVUP Server Events, Chaotic: Teleporting {player.Nickname} to {ally.Nickname}");
                                player.Position = ally.Position;
                                teleportedPlayers.Add(player);
                                teleportedPlayers.Add(ally);
                                player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.TeleportToAllyPlayerTeleportedText);
                                player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.TeleportToAllyNewAllyText);
                            }
                        }
                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Teleporting to allies event is disabled");
                        }

                        break;
                    // Simon Says Jump Event
                    case 25:
                        if (_config.SimonSaysEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Running Simon Says event");
                            if (_realSimonSaysEvent || _fakeSimonSaysEvent)
                            {
                                Log.Debug(
                                    "VVUP Server Events, Chaotic: Simon Says event is already running, returning");
                                if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                    chaoticEventCycle = 1;
                                yield break;
                            }

                            _realSimonSaysEvent = true;
                            Log.Debug("VVUP Server Events, Chaotic: Subscribing Jumping Event");
                            PlayerEvent.Jumping += Plugin.Instance.ServerEventsMainEventHandler.OnJumpingCE;
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (_config.SimonSaysAffectsScps && player.Role.Side != Side.None)
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SimonSaysJumpText);
                                else if (player.Role.Side != Side.Scp && player.Role.Side != Side.None)
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SimonSaysJumpText);
                            }

                            Timing.CallDelayed(_config.SimonSaysActionTime, () =>
                            {
                                Log.Debug("VVUP Server Events, Chaotic: Unsubscribing Jumping Event");
                                PlayerEvent.Jumping -= Plugin.Instance.ServerEventsMainEventHandler.OnJumpingCE;
                                foreach (PlayerAPI player in PlayerAPI.List)
                                {
                                    if (Plugin.Instance.ServerEventsMainEventHandler.JumpingPlayers.Contains(player))
                                        continue;
                                    if (_config.SimonSaysAffectsScps && player.Role.Side != Side.None)
                                    {
                                        player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SimonSaysPunishmentText);
                                        player.Hurt(_config.SimonSaysDamagePunishment);
                                    }
                                    else if (player.Role.Side != Side.Scp && player.Role.Side != Side.None)
                                    {
                                        player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SimonSaysPunishmentText);
                                        player.Hurt(_config.SimonSaysDamagePunishment);
                                    }
                                }
                                _realSimonSaysEvent = false;
                                Plugin.Instance.ServerEventsMainEventHandler.JumpingPlayers.Clear();
                            });
                        }
                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Simon Says event is disabled");
                        }
                        break;
                    case 26:
                        if (_config.SimonSaysEvent)
                        {
                            Log.Debug("VVUP Server Events, Chaotic: Running Someone Says event");
                            if (_realSimonSaysEvent || _fakeSimonSaysEvent)
                            {
                                Log.Debug(
                                    "VVUP Server Events, Chaotic: Simon Says event is already running, returning");
                                if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                    chaoticEventCycle = 1;
                                yield break;
                            }

                            _fakeSimonSaysEvent = true;
                            Log.Debug("VVUP Server Events, Chaotic: Subscribing Jumping Event");
                            PlayerEvent.Jumping += Plugin.Instance.ServerEventsMainEventHandler.OnJumpingCE;
                            foreach (PlayerAPI player in PlayerAPI.List)
                            {
                                if (_config.SimonSaysAffectsScps && player.Role.Side != Side.None)
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SomeoneSaysJumpText);
                                else if (player.Role.Side != Side.Scp && player.Role.Side != Side.None)
                                    player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SomeoneSaysJumpText);
                            }

                            Timing.CallDelayed(_config.SimonSaysActionTime, () =>
                            {
                                Log.Debug("VVUP Server Events, Chaotic: Unsubscribing Jumping Event");
                                PlayerEvent.Jumping -= Plugin.Instance.ServerEventsMainEventHandler.OnJumpingCE;
                                foreach (PlayerAPI player in PlayerAPI.List)
                                {
                                    if (!Plugin.Instance.ServerEventsMainEventHandler.JumpingPlayers.Contains(player))
                                        continue;
                                    if (_config.SimonSaysAffectsScps && player.Role.Side != Side.None)
                                    {
                                        player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SomeoneSaysPunishmentText);
                                        player.Hurt(_config.SimonSaysDamagePunishment);
                                    }
                                    else if (player.Role.Side != Side.Scp && player.Role.Side != Side.None)
                                    {
                                        player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.SomeoneSaysPunishmentText);
                                        player.Hurt(_config.SimonSaysDamagePunishment);
                                    }
                                }

                                _fakeSimonSaysEvent = false;
                                Plugin.Instance.ServerEventsMainEventHandler.JumpingPlayers.Clear();
                            });
                        }
                        else
                        {
                            if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                                chaoticEventCycle = 1;
                            Log.Debug("VVUP Server Events, Chaotic: Simon Says event is disabled");
                        }
                        break;
                    // Default Case
                    default:
                    {
                        Log.Debug("VVUP Server Events, Chaotic: Something out of range has occured");
                        if (_config.ChaoticEventRerollIfASpecificEventIsDisabled)
                            chaoticEventCycle = 1;
                        break;
                    }
                }

                Log.Debug($"VVUP Server Events, Chaotic: The next event will run in {chaoticEventCycle} seconds");
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
                        $"VVUP Server Events, Chaotic: Warhead time hasn't reached {_config.FakeAutoNukeTimeOut} yet. Waiting half a second and checking again");
                    Log.Debug($"VVUP Server Events, Chaotic: Detonation Time is {Warhead.DetonationTimer}");
                }
                else
                {
                    Log.Debug("VVUP Server Events, Chaotic: Time has been reached, stopping warhead");
                    Warhead.IsLocked = false;
                    if (_config.FakeAutoNukeRestoresOldTime)
                        Warhead.DetonationTimer = _previousWarheadTime;
                    Warhead.Stop();
                    foreach (PlayerAPI player in PlayerAPI.List)
                    {
                        Log.Debug($"VVUP Server Events, Chaotic: Displaying the fake out message to {player.Nickname}");
                        player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.FakeAutoNukeFakeoutText);
                    }

                    _ceFakeWarheadEvent = false;
                    yield break;
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        private static IEnumerator<float> RapidFireTelsa( /*TriggeringTeslaEventArgs ev*/)
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

                Log.Debug(
                    $"{teslaGate.ActivationTime}, {teslaGate.TriggerRange}, {teslaGate.CooldownTime}, {teslaGate.IdleRange}");

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
                    Log.Debug(
                        "VVUP Server Events, Chaotic: Going from top down, modified activation time, idle range, trigger range, cooldown time");
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
                        "VVUP Server Events, Chaotic: Round time hasn't reached the threshold to end rapid fire teslas event, waiting half a second and checking again");
                    Log.Debug(
                        $"VVUP Server Events, Chaotic: Current Round Time is: {Round.ElapsedTime.TotalSeconds}. Threshold is: {roundTimeFromEventStart + _config.RapidFireTeslaEventTiming}");
                    Log.Debug(
                        $"VVUP Server Events, Chaotic: Act {modifiedActivationTime}, Idl {modifiedIdleRange}, Tri {modifiedTriggerRange}, Coo {modifiedCooldownTime}");
                }
                else
                {
                    Log.Debug("VVUP Server Events, Chaotic: Time threshold has been reached, ending event");
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
            Log.Debug($"VVUP Server Events, Chaotic: Waiting for {_config.RouterKickingSimulatorStartWaitTime} before starting properly");
            Timing.WaitForSeconds(_config.RouterKickingSimulatorStartWaitTime);
            for (;;)
            {
                if (routerKickCount != _config.RouterKickingSimulatorRouterKickAmount)
                {
                    foreach (PlayerAPI player in PlayerAPI.List)
                        if (player.Role.Team != Team.Dead)
                        {
                            Log.Debug($"VVUP Server Events, Chaotic: {player.Nickname} is not dead, logging position");
                            playerPositions[player.Id] = player.Position;
                        }

                    Log.Debug($"VVUP Server Events, Chaotic: Waiting {_config.RouterKickingSimulatorLagTime} seconds");
                    Timing.WaitForSeconds(_config.RouterKickingSimulatorLagTime);

                    foreach (PlayerAPI player in PlayerAPI.List)
                    {
                        if (playerPositions.TryGetValue(player.Id, out Vector3 position))
                        {
                            Log.Debug($"VVUP Server Events, Chaotic: Getting {player.Nickname} previously logged position");
                            player.Position = position;
                        }
                        else
                            Log.Debug(
                                $"VVUP Server Events, Chaotic: {player.Nickname} isn't in the list. Most likely due to not being connected or alive during the last cycle.");
                    }

                    routerKickCount++;
                    Log.Debug($"VVUP Server Events, Chaotic: Adding to the router kick count, which is now {routerKickCount}");
                }
                else
                {
                    Log.Debug("VVUP Server Events, Chaotic: Amount of router kicks has been reached, ending event");
                    foreach (PlayerAPI player in PlayerAPI.List)
                    {
                        Log.Debug($"VVUP Server Events, Chaotic: Showing Router Kicking Simulator end message to {player.Nickname}");
                        player.Broadcast((ushort)_config.BroadcastDisplayTime, _config.RouterKickingSimulatorEndBroadcast);
                    }

                    if (_config.RouterKickingSimulatorDisablesElevators)
                    {
                        Log.Debug("VVUP Server Events, Chaotic: Unlocking elevator doors");
                        foreach (Door door in Door.List)
                            if (door is ElevatorDoor { IsLocked: true } elevatorDoor)
                                elevatorDoor.ChangeLock(DoorLockType.AdminCommand);
                    }

                    if (_config.RouterKickingSimulatorDisablesDecomAndNuke)
                    {
                        Log.Debug("VVUP Server Events, Chaotic: Waiting a fixed 15 seconds before reenabling decom & nuke");
                        Timing.WaitForSeconds(15);
                        Log.Debug("VVUP Server Events, Chaotic: 15 seconds has passed, reenabling decom and nuke");
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

                Log.Debug($"VVUP Server Events, Chaotic: Waiting {_config.RouterKickingSimulatorTimeBetweenRouterKicks} seconds");
                yield return Timing.WaitForSeconds(_config.RouterKickingSimulatorTimeBetweenRouterKicks);
            }
        }

        private static readonly List<ItemType> WeaponTypes = new()
        {
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
        };

        private static bool IsWeapon(Item item)
        {
            return WeaponTypes.Contains(item.Type);
        }

        private static PlayerAPI GetRandomPlayerFBI()
        {
            Log.Debug("VVUP Server Events, Chaotic: Getting a random player for the FBI Open Up Chaos Event");
            List<PlayerAPI> fbiOpenUpPossibleTargets = PlayerAPI.List.Where(p =>
                p.Role != (RoleTypeId)Team.FoundationForces || p.Role != (RoleTypeId)Team.Scientists ||
                p.Role != (RoleTypeId)Team.Dead).ToList();
            if (fbiOpenUpPossibleTargets.Count == 0)
                return null;
            int index = Base.GetRandomNumber.GetRandomInt(fbiOpenUpPossibleTargets.Count);
            return fbiOpenUpPossibleTargets[index];
        }

        private static string GetNatoName(int randomUnit)
        {
            Log.Debug(
                "VVUP Server Events, Chaotic: Getting a Nato Name (even though its not called the Nato alphabet, its called the phonetic alphabet");
            Dictionary<int, string> natoAlphabet = new Dictionary<int, string>()
            {
                { 1, "ALPHA" },
                { 2, "BRAVO" },
                { 3, "CHARLIE" },
                { 4, "DELTA" },
                { 5, "ECHO" },
                { 6, "FOXTROT" },
                { 7, "GOLF" },
                { 8, "HOTEL" },
                { 9, "INDIA" },
                { 10, "JULIET" },
                { 11, "KILO" },
                { 12, "LIMA" },
                { 13, "MIKE" },
                { 14, "NOVEMBER" },
                { 15, "OSCAR" },
                { 16, "PAPA" },
                { 17, "QUEBEC" },
                { 18, "ROMEO" },
                { 19, "SIERRA" },
                { 20, "TANGO" },
                { 21, "UNIFORM" },
                { 22, "VICTOR" },
                { 23, "WHISKEY" },
                { 24, "XRAY" },
                { 25, "YANKEE" },
                { 26, "ZULU" },
            };
            return natoAlphabet[randomUnit];
        }

        private static string GetNatoLetter(int randomUnit)
        {
            Log.Debug("VVUP Server Events, Chaotic: Getting a random letter");
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
            Log.Debug("VVUP Server Events, Chaotic: Running Mtf Fakeout Cassie handle");
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
                    int randomNatoLetter = Base.GetRandomNumber.GetRandomInt(1, 26);
                    int randomNatoNumber = Base.GetRandomNumber.GetRandomInt(2, 20);
                    cassieMessage = cassieMessage.Replace("{designation}",
                        $"nato_{GetNatoLetter(randomNatoLetter)} {randomNatoNumber}");
                    cassieText = cassieText.Replace("{designation}",
                        GetNatoName(randomNatoLetter) + " " + randomNatoNumber);
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

                    int randomNatoLetter = Base.GetRandomNumber.GetRandomInt(1,  27);
                    int randomNatoNumber = Base.GetRandomNumber.GetRandomInt(2, 21);
                    cassieMessage = cassieMessage.Replace("{designation}",
                        $"nato_{GetNatoLetter(randomNatoLetter)} {randomNatoNumber}");
                    cassieText = cassieText.Replace("{designation}",
                        GetNatoName(randomNatoLetter) + " " + randomNatoNumber);
                }
                LabApi.Features.Wrappers.Cassie.Message(cassieMessage, cassieText);
            }
        }

        public static void EndEvent()
        {
            if (!_ceStarted) return;
            _ceStarted = false;
            Log.Debug("VVUP Server Events, Chaotic: Killing main Coroutine Handle for Chaotic Events");
            Timing.KillCoroutines(_choaticHandle);
            Plugin.ActiveEvent -= 1;
            if (_ceMedicalItemEvent)
            {
                Log.Debug("VVUP Server Events, Chaotic: Removing On Using Item Completed Event");
                PlayerEvent.UsingItemCompleted -= Plugin.Instance.ServerEventsMainEventHandler.OnUsingMedicalItemCE;
                _ceMedicalItemEvent = false;
            }

            if (_ceFakeWarheadEvent)
            {
                Log.Debug("VVUP Server Events, Chaotic: Killing fake warhead coroutine");
                Timing.KillCoroutines(_fakeWarheadHandle);
                _ceFakeWarheadEvent = false;
            }

            if (_ceRapidFireTeslas)
            {
                Log.Debug("VVUP Server Events, Chaotic: Killing rapid fire teslas coroutine");
                Timing.KillCoroutines(_rapidFireTeslas);
                _ceRapidFireTeslas = false;
            }

            if (_ceRouterKickingSimulator)
            {
                Log.Debug("VVUP Server Events, Chaotic: Killing router kicking coroutine");
                Timing.KillCoroutines(_routerKickingSimulator);
                _ceRouterKickingSimulator = false;
            }
            
            if (_realSimonSaysEvent || _fakeSimonSaysEvent)
            {
                PlayerEvent.Jumping -= Plugin.Instance.ServerEventsMainEventHandler.OnJumpingCE;
                _realSimonSaysEvent = false;
                _fakeSimonSaysEvent = false;
            }
        }
    }
}