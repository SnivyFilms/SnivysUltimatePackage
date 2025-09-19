using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using UnityEngine;
using PlayerLab = LabApi.Features.Wrappers.Player;

// ReSharper disable InconsistentNaming

namespace VVUP.ServerEvents.ServerEventsEventHandlers
{
    
    public class ServerEventsMainEventHandler
    {
        public Plugin Plugin;
        public ServerEventsMainEventHandler(Plugin plugin) => Plugin = plugin;
        
        private static int _activatedGenerators;
        private static float _PHEScale;
        private static float _PHENewHealth;
        private static float _PHELastKnownHeath;
        private static float _PHELastKnownScale;
        public List<Player> JumpingPlayers;
        
        //Ending round
        public void OnEndingRound(RoundEndedEventArgs ev)
        {
            Log.Debug("VVUP Server Events: Checking if an event is active at round end");
            EndEvents();
        }
        
        //Waiting for Players
        public void OnWaitingForPlayers()
        {
            Log.Debug("VVUP Server Events: Checking if an event is active at waiting for players");
            EndEvents();
        }

        //Stop Events Command
        public static void StopEventsCommand()
        {
            Log.Debug("VVUP Server Events: Killing events due to the stop command being used");
            EndEvents();
        }

        //Ends Events method, On Round End, On Waiting For Players, and Stop Commands points here
        private static void EndEvents()
        {
            Log.Debug("VVUP Server Events: Checking again if there's events active");
            if (Plugin.ActiveEvent == 0) return;
            Log.Debug("VVUP Server Events: Disabling Event Handlers, Clearing Generator Count");
            _activatedGenerators = 0;
            BlackoutEventHandlers.EndEvent();
            PeanutHydraEventHandlers.EndEvent();
            PeanutInfectionEventHandlers.EndEvent();
            VariableLightsEventHandlers.EndEvent();
            ShortEventHandlers.EndEvent();
            FreezingTemperaturesEventHandlers.EndEvent();
            ChaoticEventHandlers.EndEvent();
            NameRedactedEventHandlers.EndEvent();
            AfterHoursEventHandlers.EndEvent();
            //SnowballsVsScpsEventHandlers.EndEvent();
            GravityEventHandlers.EndEvent();
            ItemRandomizerEventHandlers.EndEvent();
            NoSpectatingPlayersEventHandlers.EndEvent();
            Plugin.ActiveEvent = 0;
        }

        //On round start, basically to see if events can start randomly
        public void OnRoundStart()
        {
            Log.Debug("VVUP Server Events: Checking if Random Events config is set to true");
            if (!Plugin.Instance.Config.RandomlyStartingEvents) return;
            
            int chance = Base.GetRandomNumber.GetRandomInt(101);
            Log.Debug($"VVUP Server Events: Chance is {chance}, comparing to the chance to the defined chance {Plugin.Instance.Config.RandomlyStartingEvents}");
            if (chance < Plugin.Instance.Config.RandomEventStartingChance)
            {
                Log.Debug("VVUP Server Events: Getting the list of events that are able to be randomly started");
                List<string> events = Plugin.Instance.Config.RandomEventsAllowedToStart;
                Log.Debug("VVUP Server Events: Getting a random event that is defined");
                string selectedEvent = events[Base.GetRandomNumber.GetRandomInt(events.Count)];
                Log.Debug($"VVUP Server Events: Random event selected: {selectedEvent}");
                switch (selectedEvent)
                {
                    case "Blackout":
                        Log.Debug("VVUP Server Events: Activating Blackout Event");
                        var blackoutEventHandlers = new BlackoutEventHandlers();
                        break;
                    case "173Infection":
                        Log.Debug("VVUP Server Events: Activating Peanut Infection Event");
                        var infectionEventHandlers = new PeanutInfectionEventHandlers();
                        break;
                    case "173Hydra":
                        Log.Debug("VVUP Server Events: Activating Peanut Hydra Event");
                        var hydraEventHandlers = new PeanutHydraEventHandlers();
                        break;
                    case "Chaotic":
                        Log.Debug("VVUP Server Events: Activating Chaotic Event");
                        var chaoticHandlers = new ChaoticEventHandlers();
                        break;
                    case "Short":
                        Log.Debug("VVUP Server Events: Activating Short People Event");
                        var shortEventHandlers = new ShortEventHandlers();
                        break;
                    case "FreezingTemps":
                        Log.Debug("VVUP Server Events: Activating Freezing Temperatures Event");
                        var freezingTemperaturesHandlers = new FreezingTemperaturesEventHandlers();
                        break;
                    case "NameRedacted":
                        Log.Debug("VVUP Server Events: Activating Name Redacted Event");
                        var nameRedactedHandler = new NameRedactedEventHandlers();
                        break;
                    case "VariableLights":
                        Log.Debug("VVUP Server Events: Activating Variable Lights Event");
                        var variableEventHandlers = new VariableLightsEventHandlers();
                        break;
                    case "AfterHours":
                        Log.Debug("VVUP Server Events: Activating Variable Lights Event");
                        var afterHoursEventHandlers = new AfterHoursEventHandlers();
                        break;
                    case "LowGravity":
                        Log.Debug("VVUP Server Events: Activating Low Gravity Event");
                        var gravityEventHandlers = new GravityEventHandlers();
                        break;
                    case "ItemRandomizer":
                        Log.Debug("VVUP Server Events: Activating No Spectating Players Event");
                        var itemRandomizerEventHandlers = new ItemRandomizerEventHandlers();
                        break;
                    //case "SnowballsVsScps":
                        //Log.Debug("VVUP Server Events: Activating Snowballs Vs Scps Event");
                        // var snowballsVsScpsEventHandlers = new SnowballsVsScpsEventHandlers();
                        //break;
                    case "NoSpectatingPlayers":
                        Log.Debug("VVUP Server Events: Activating No Spectating Players Event");
                        var noSpectatingPlayersEventHandlers = new NoSpectatingPlayersEventHandlers();
                        break;
                    default:
                        Log.Warn($"VVUP Server Events: Unknown event: {selectedEvent}");
                        Log.Warn("VVUP Server Events: Valid Events: Valid options: Blackout, 173Infection, 173Hydra, Chaotic, Short, FreezingTemps, NameRedacted, VariableLights, LowGravity, ItemRandomizer, NoSpectatingPlayers");
                        Log.Warn("VVUP Server Events: If this error randomly appears and you are sure you put in a valid event, please let the developer know as soon as possible");
                        break;
                }
            }
            else
                Log.Debug("VVUP Server Events: No random event was triggered.");
        }
        
        //Blackout
        public void OnGeneratorEngagedBOE(GeneratorActivatingEventArgs ev)
        {
            Log.Debug("VVUP Server Events: Adding amount of generators to count");
            _activatedGenerators = Generator.Get(GeneratorState.Engaged).Count();
            Log.Debug("Checking if generators is 3");
            if (_activatedGenerators == 3)
            {
                Log.Debug("VVUP Server Events: Disabling Blackout Event");
                BlackoutEventHandlers.EndEvent();
                Plugin.ActiveEvent -= 1;
                _activatedGenerators = 0;
            }
        }
        
        // Peanut Infection
        public void OnKillingPIE(DiedEventArgs ev)
        {
            Log.Debug("VVUP Server Events: Checking if the killer was 173");
            if (ev.Attacker.Role == RoleTypeId.Scp173 && ev.DamageHandler.Type == DamageType.Scp173)
            {
                Log.Debug("VVUP Server Events: Setting the killed to 173");
                Timing.CallDelayed(0.5f, () => ev.Player.Role.Set(RoleTypeId.Scp173, SpawnReason.ForceClass, RoleSpawnFlags.None));
            }
        }
        
        // Peanut Hydra
        public void OnDyingPHE(DyingEventArgs ev)
        {
            Log.Debug("VVUP Server Events: Checking if the died is SCP-173");
            if (ev.Player.Role != RoleTypeId.Scp173) return;
            _PHELastKnownHeath = ev.Player.Health;
            _PHELastKnownScale = ev.Player.Scale.y;
        }
        
        public void OnDiedPHE(DiedEventArgs ev)
        {
            if (ev.TargetOldRole != RoleTypeId.Scp173) return;
            //Get the player who died and set them back as 173 
            Log.Debug("VVUP Server Events: Get the player who died and set them back as 173");
            ev.Player.Role.Set(RoleTypeId.Scp173, SpawnReason.ForceClass, RoleSpawnFlags.None);
            //calculate the new scale and health
            Log.Debug("VVUP Server Events: Calculating the new scale and health");
            _PHEScale = Mathf.Max(0.1f, _PHELastKnownScale / 2);
            _PHENewHealth = _PHELastKnownHeath / 2;
            //apply them to the formerly dead player
            Log.Debug("VVUP Server Events: Applying them to the formerly dead player");
            ev.Player.Health = Mathf.Max(_PHENewHealth, 1);
            ev.Player.Scale.Set(_PHEScale, _PHEScale, _PHEScale);
            //Get a random spectator and set them as a duplicate 173
            Log.Debug("VVUP Server Events: Getting a random spectator and set them as a duplicate 173");
            Player newPlayer = Base.GetRandomSpectator.GetSpectator();
            switch (newPlayer)
            {
                case null when PeanutHydraEventHandlers.Config.UseAttackersIfNeeded:
                    Log.Debug("VVUP Server Events: No spectators found to become the new SCP-173, using attacker...");
                    newPlayer = ev.Attacker;
                    break;
                case null:
                    Log.Debug("VVUP Server Events: No spectators found to become the new SCP-173");
                    return;
            }
            newPlayer.Role.Set(RoleTypeId.Scp173, SpawnReason.ForceClass, RoleSpawnFlags.None);
            newPlayer.Position = ev.Player.Position;
            newPlayer.Health = _PHENewHealth;
            newPlayer.Scale = new Vector3(_PHEScale, _PHEScale, _PHEScale);
        }
        public void OnRoleSwapSE(ChangingRoleEventArgs ev)
        {
            Log.Debug($"VVUP Server Events: Setting {ev.Player.Nickname} size to {ShortEventHandlers.GetPlayerSize()}");
            ev.Player.Scale = new Vector3(ShortEventHandlers.GetPlayerSize(), ShortEventHandlers.GetPlayerSize(), ShortEventHandlers.GetPlayerSize());
        }
        
        //Chaos Event
        public void OnUsingMedicalItemCE(UsingItemCompletedEventArgs ev)
        {
            Log.Debug("VVUP Server Events: Checking if the item is used was a medical item of some sort");
            if (ev.Usable.Type is ItemType.Adrenaline or ItemType.Painkillers or ItemType.Medkit or ItemType.SCP500)
            {
                Log.Debug("VVUP Server Events: Doing a half half chance to see if an effect should be applied");
                int chance = Base.GetRandomNumber.GetRandomInt(0, 2);
                if (chance == 1)
                    return;
                Log.Debug("VVUP Server Events: Chance passed, getting a random effect");
                chance = Base.GetRandomNumber.GetRandomInt(1, 42);
                switch (chance)
                {
                    case 1:
                        break;
                    case 2:
                        ev.Player.EnableEffect(EffectType.Asphyxiated, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 3:
                        ev.Player.EnableEffect(EffectType.Bleeding, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 4:
                        ev.Player.EnableEffect(EffectType.Blinded, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 5:
                        ev.Player.EnableEffect(EffectType.Burned, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 6:
                        ev.Player.EnableEffect(EffectType.Concussed, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 7:
                        ev.Player.EnableEffect(EffectType.Corroding, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 8:
                        ev.Player.EnableEffect(EffectType.Deafened, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 9:
                        ev.Player.EnableEffect(EffectType.Decontaminating, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 10:
                        ev.Player.EnableEffect(EffectType.Disabled, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 11:
                        ev.Player.EnableEffect(EffectType.Ensnared, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 12:
                        ev.Player.EnableEffect(EffectType.Exhausted, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 13:
                        ev.Player.EnableEffect(EffectType.Flashed, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 14:
                        ev.Player.EnableEffect(EffectType.Ghostly, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 15:
                        ev.Player.EnableEffect(EffectType.Hemorrhage, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 16:
                        ev.Player.EnableEffect(EffectType.Hypothermia, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 17:
                        ev.Player.EnableEffect(EffectType.Invigorated, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 18:
                        ev.Player.EnableEffect(EffectType.Invisible, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 19:
                        ev.Player.EnableEffect(EffectType.Poisoned, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 20:
                        ev.Player.EnableEffect(EffectType.Scp207, (byte)Base.GetRandomNumber.GetRandomInt(1, 4), Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 21:
                        ev.Player.EnableEffect(EffectType.Scp1853, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 22:
                        ev.Player.EnableEffect(EffectType.Slowness, (byte)Base.GetRandomNumber.GetRandomInt(1, 255), Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 23:
                        ev.Player.EnableEffect(EffectType.Stained, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 24:
                        ev.Player.EnableEffect(EffectType.Traumatized, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 25:
                        ev.Player.EnableEffect(EffectType.Vitality, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 26:
                        ev.Player.EnableEffect(EffectType.AmnesiaItems, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 27:
                        ev.Player.EnableEffect(EffectType.AmnesiaVision, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 28:
                        ev.Player.EnableEffect(EffectType.AntiScp207, (byte)Base.GetRandomNumber.GetRandomInt(1, 4), Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 29:
                        ev.Player.EnableEffect(EffectType.BodyshotReduction, (byte)Base.GetRandomNumber.GetRandomInt(1, 5), Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 30:
                        ev.Player.EnableEffect(EffectType.CardiacArrest, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 31:
                        ev.Player.EnableEffect(EffectType.DamageReduction, (byte)Base.GetRandomNumber.GetRandomInt(1, 256), Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 32:
                        ev.Player.EnableEffect(EffectType.FogControl, (byte)Base.GetRandomNumber.GetRandomInt(0, 8), Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 33:
                        ev.Player.EnableEffect(EffectType.MovementBoost, (byte)Base.GetRandomNumber.GetRandomInt(1, 256), Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 34:
                        ev.Player.EnableEffect(EffectType.PocketCorroding, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 35:
                        ev.Player.EnableEffect(EffectType.RainbowTaste, (byte)Base.GetRandomNumber.GetRandomInt(1, 4), Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 36:
                        ev.Player.EnableEffect(EffectType.SeveredHands, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 37:
                        ev.Player.EnableEffect(EffectType.SilentWalk, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 38:
                        ev.Player.EnableEffect(EffectType.SinkHole, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 39:
                        ev.Player.EnableEffect(EffectType.SpawnProtected, 1, Base.GetRandomNumber.GetRandomFloat());
                        break;
                    case 40:
                        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                        grenade.FuseTime = Base.GetRandomNumber.GetRandomFloat();
                        grenade.SpawnActive(ev.Player.Position);
                        break;
                    case 41:
                        ev.Player.Kill("You used a medical item unsafely");
                        break;
                }

            }
        }
        
        public void OnTeslaActivationAh(TriggeringTeslaEventArgs ev)
        {
            ev.IsAllowed = AfterHoursEventHandlers.AhTeslaAllowed;
        }
        
        public void OnJumpingCE(JumpingEventArgs ev)
        {
            Log.Debug($"VVUP Server Events, Chaotic: {ev.Player.Nickname} jumped. Adding to list");
            JumpingPlayers.Add(ev.Player);
        }

        /*public void OnDyingSvs(DyingEventArgs ev)
        {
            Log.Debug($"VVUP Server Events: Snowballs Vs Scps, Setting {ev.Player.Nickname} to Overwatch because they died");
            Timing.CallDelayed(0.5f, () => ev.Player.Role.Set(RoleTypeId.Overwatch));
            SnowballsVsScpsEventHandlers.PlayersInOverwatchFromEvent.Add(ev.Player);
        }*/
        
        public void OnRoleSwapGE(ChangingRoleEventArgs ev)
        {
            if (ev.Player == null)
                return;
            if (ev.Player.NetworkIdentity == null)
                return;
            Log.Debug($"VVUP Server Events: Setting {ev.Player.Nickname} size to {Plugin.Instance.Config.GravityConfig.GravityChanges}");
            PlayerLab.Get(ev.Player.NetworkIdentity).Gravity = Plugin.Instance.Config.GravityConfig.GravityChanges;
        }
        
        public void OnPickingUpItemIR(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null || ev.Pickup == null)
                return;
            bool isCustomItem = CustomItem.TryGet(ev.Pickup, out _);
            ev.IsAllowed = false;
            ev.Pickup.Destroy();
            if (isCustomItem)
            {
                Log.Debug("VVUP Server Events, Item Randomizer: Player picked up a custom item, giving a random custom item.");
                GetRandomCustomItem().Give(ev.Player);
            }
            else
            {
                Log.Debug("VVUP Server Events, Item Randomizer: Player picked up a base item, giving a random base item.");
                ev.Player.AddItem(GetRandomBaseItem());
            }
        }

        public void OnDroppingItemIR(DroppingItemEventArgs ev)
        {
            if (ev.Player == null || ev.Item == null)
                return;
            bool isCustomItem = CustomItem.TryGet(ev.Item, out _);
            ev.IsAllowed = false;
            ev.Player.RemoveItem(ev.Item);
            if (isCustomItem)
            {
                Log.Debug("VVUP Server Events, Item Randomizer: Player dropped a custom item, dropping a random custom item.");
                GetRandomCustomItem().Spawn(ev.Player.Position);
            }
            else
            {
                Log.Debug("VVUP Server Events, Item Randomizer: Player dropped a base item, dropping a random base item.");
                Pickup.CreateAndSpawn(GetRandomBaseItem(), ev.Player.Position);
            }
        }
        
        private ItemType GetRandomBaseItem()
        {
            Array itemTypes = Enum.GetValues(typeof(ItemType));
            int index = Base.GetRandomNumber.GetRandomInt(itemTypes.Length);
            return (ItemType)itemTypes.GetValue(index);
        }
        private CustomItem GetRandomCustomItem()
        {
            List<CustomItem> customItems = CustomItem.Registered.ToList();
            return customItems[Base.GetRandomNumber.GetRandomInt(customItems.Count)];
        }
    }
}