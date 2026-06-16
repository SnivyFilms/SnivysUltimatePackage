using System.ComponentModel;
using JetBrains.Annotations;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class ChaoticConfig
    {
        public string StartEventCassieMessage { get; set; } = "Facility is unstable";
        public string StartEventCassieText { get; set; } = "Facility is unstable (Chaos Event)";

        [Description("Determines how long before the next chaos event (in seconds)")]
        public float TimeForChaosEvent { get; set; } = 60;

        [Description("Should the Chaotic Event reroll if a specific event is disabled?")]
        public bool ChaoticEventRerollIfASpecificEventIsDisabled { get; set; } = true;

        [Description(
            "The time for Broadcasts to be shown on the screen (this applies to all broadcasts that the Chaotic Event uses)")]
        public float BroadcastDisplayTime { get; set; } = 10;

        [Description("Should the item steal chaos event run?")]
        public bool ItemStealEvent { get; set; } = true;

        [Description("What should the broadcast say when players items has been stolen")]
        public string ItemStolenText { get; set; } = "It has appears that your items has been stolen";

        [Description("Should give random items chaos event run?")]
        public bool GiveRandomItemEvent { get; set; } = true;

        [Description("What should the broadcast say when an item is given")]
        public string GiveRandomItemText { get; set; } = "It has appears that you have obtained an item";

        [Description("Should random items only be regular items or use custom items as well?")]
        public bool GiveRandomItemCustomitems { get; set; } = false;

        [Description("Should random teleport chaos event run?")]
        public bool RandomTeleportEvent { get; set; } = true;

        [Description(
            "If the Random Teleport Event is active, should players be teleported to Light after decomtamination?")]
        public bool TeleportToLightAfterDecom { get; set; } = false;

        [Description(
            "If the Random Teleport Event is active, should players be teleported into the facility after the warhead detonated?")]
        public bool TeleportToFacilityAfterNuke { get; set; } = false;

        [Description("What should the broadcast say when a player gets teleported?")]
        public string RandomTeleportText { get; set; } = "Whoops, my bad";

        [Description("Should the fake autonuke chaos event run?")]
        public bool FakeAutoNuke { get; set; } = true;

        [Description(
            "What should the broadcast say for when the fake auto nuke starts (this should match your real autonuke text to make it more conviencing")]
        public string FakeAutoNukeStartText { get; set; } = "Autonuke has started";

        [Description("What should the broadcast say for when it is reviealed that the auto nuke was fake")]
        public string FakeAutoNukeFakeoutText { get; set; } = "Get pranked";

        [Description(
            "What should be the time before the nuke goes off before its shown to be a fake, note that anything below 10 seconds wont work")]
        public float FakeAutoNukeTimeOut { get; set; } = 15f;

        [Description("Should the old nuke time be restored after the fake out? Otherwise it will be at 30 seconds")]
        public bool FakeAutoNukeRestoresOldTime { get; set; } = true;

        [Description("Should the remove weapons chaos event run?")]
        public bool RemoveWeaponsEvent { get; set; } = true;

        [Description("What should the broadcast text say when a weapon is taken away?")]
        public string RemoveWeaponsText { get; set; } = "I hope you didnt plan on entering a fight soon";

        [Description("Should the give random weapon event be active?")]
        public bool GiveRandomWeaponsEvent { get; set; } = true;

        [Description("Should the random weapon giving only apply to people who dont have weapons?")]
        public bool GiveRandomWeaponsToUnarmedPlayers { get; set; } = true;

        [Description("What should the broadcast text say when a weapon is given?")]
        public string GiveRandomWeaponsText { get; set; } = "Here have a weapon";

        [Description("What should the broadcast say if the player's inventory is full and cannot recieve a weapon")]
        public string GiveRandomWeaponsTextFail { get; set; } = "No free weaponry for you, bawomp";

        [Description("Should all weapons be up for grabs?")]
        public bool GiveAllRandomWeapons { get; set; } = true;

        [Description("If giving all weapons is false, what weapons can be given?")]
        [CanBeNull]
        public List<ItemType> GiveRandomWeaponsDefined { get; set; } = new()
        {
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunCrossvec,
        };

        [Description("Should the death match event be active?")]
        public bool DeathMatchEvent { get; set; } = true;

        [Description("What should the broadcast say when players health is reduced?")]
        public string DeathMatchText { get; set; } = "*Sneezes* Sorry I hope I didnt mess anything up";

        [Description("Should SCPs be affected by this as well?")]
        public bool DeathMatchEventAffectsSCPs { get; set; } = false;

        [Description("What health value should players health be set at?")]
        public float DeathMatchHealth { get; set; } = 1f;

        [Description("Should the Chaos Event be able to enable other events?")]
        public bool ChaosEventEnablesOtherEvents { get; set; } = true;

        [Description("Should the other events that get activated end when the next cycle comes along?")]
        public bool ChaosEventEndsOtherEvents { get; set; } = false;

        [Description("Should the FBI Open Up Event be active?")]
        public bool FBIOpenUpEvent { get; set; } = true;

        [Description("What should the broadcast say to the target that's going to be teleported too")]
        public string FBIOpenUpTargetText { get; set; } =
            "The FBI is rapidly approaching your location, prepare yourself";

        [Description("What should the broadcast say to the MTF & Guards that are going to be teleported to a target")]
        public string FBIOpenUpMTFText { get; set; } = "Fight or Flight.";

        [Description("How long should it take before the foundation gets teleported to the target?")]
        public float FBITeleportTime { get; set; } = 5f;

        [Description("Should the grenade feet event be active?")]
        public bool GrenadeFeetEvent { get; set; } = true;

        [Description("Should grenade fuses be random?")]
        public bool GrenadeFeetRandomFuse { get; set; } = true;

        [Description("How long should the fuse on the grenades be if it's not random?")]
        public float GrenadeFeetFuse { get; set; } = 5f;

        [Description("What should the broadcast be before the grenades drop on people?")]
        public string GrenadeFeetText { get; set; } = "You might want to be careful on where you step";

        [Description("Should unsafe medical items event be active?")]
        public bool UnsafeMedicalItemsEvent { get; set; } = true;

        [Description("What should the broadcast be for unsafe medical items?")]
        public string UnsafeMedicalItemsText { get; set; } = "Use medical items with care";

        [Description("Should unsafe medical items use a random time for the event?")]
        public bool UnsafeMedicalItemsUseRandomTime { get; set; } = true;

        [Description("If random time for the event is off, how long should Unsafe Medical Items run for?")]
        public float UnsafeMedicalItemsFixedTime { get; set; } = 30f;

        [Description("What should the broadcast be to inform players it's safe to use medical items again?")]
        public string UnsafeMedicalItemsSafeToUseText { get; set; } = "It's safe to use medical items again";

        [Description("Should fakeout announcements for respawn waves run?")]
        public bool FakeoutRespawnAnnouncementsEvent { get; set; } = true;

        [Description("Should fakeout announcements for MTF be used?")]
        public bool FakeoutRespawnAnnouncementsMTFAllow { get; set; } = true;

        [Description("Should fakeout announcements for Chaos Insurgency be used?")]
        public bool FakeoutRespawnAnnouncementsChaosAllow { get; set; } = false;

        [Description("Should fakeout announcements for Serpents Hand be used?")]
        public bool FakeoutRespawnAnnouncementsSerpentsAllow { get; set; } = false;

        [Description("Should fakeout announcements for UIU be used?")]
        public bool FakeoutRespawnAnnouncementsUIUAllow { get; set; } = false;

        [Description("Should the MTF announcement be used as a fall back if other fakeouts are disabled")]
        public bool FakeoutRespawnAnnouncementsMTFFallback { get; set; } = false;

        [Description("What should the fakeout cassie announcement say for MTF")]
        public string FakeoutRespawnAnnouncementsMTFAliveSCPSCassie { get; set; } =
            "MTFUnit epsilon 11 designated {designation} hasentered AwaitingRecontainment {scpnum}";

        public string FakeoutRespawnAnnouncementsMTFSCPSDeadCassie { get; set; } =
            "MTFUnit epsilon 11 designated {designation} hasentered NoSCPsLeft";

        [Description("What should the fakeout cassie text say for MTF")]
        public string FakeoutRespawnAnnouncementsMTFAliveSCPSCassieText { get; set; } =
            "Mobile Task Force Unit Espilon 11, designated {designation} has entered the facility. All remaining personnel are advised to proceed with standard evacuation protocols until a MTF squad reaches your destination. awaiting recontainment of {scpnum}.";

        public string FakeoutRespawnAnnouncementsMTFSCPSDeadCassieText { get; set; } =
            "Mobile Task Force Unit Espilon 11, designated {designation} has entered the facility. All remaining personnel are advised to proceed with standard evacuation protocols until a MTF squad reaches your destination. Substantial threat remains within the facility - exercise caution.";

        [Description("What should the fakeout cassie announcement say for Chaos")]
        public string FakeoutRespawnAnnouncementsChaosCassie { get; set; } =
            "Warning . Military Personnel has entered the facility . Designated as, Chaos Insurgency.";

        [Description("What should the fakeout cassie text say for Chaos")]
        public string FakeoutRespawnAnnouncementsChaosCassieText { get; set; } =
            "Warning. Military Personnel has entered the facility. Designated as, <color=green>Chaos Insurgency</color>.";

        [Description("What should the fakeout cassie announcement say for Serpents")]
        public string FakeoutRespawnAnnouncementsSerpentsCassie { get; set; } = "Serpents Hand hasentered";

        [Description("What should the fakeout cassie text say for Serpents")]
        public string FakeoutRespawnAnnouncementsSerpentsCassieText { get; set; } =
            "Serpents Hand has entered the facility";

        [Description("What should the fakeout cassie announcement say for UIU")]
        public string FakeoutRespawnAnnouncementsUIUAliveSCPSCassie { get; set; } =
            "The U I U Squad designated {designation} HasEntered AllRemaining AwaitingRecontainment {scpnum}";

        public string FakeoutRespawnAnnouncementsUIUSCPSDeadCassie { get; set; } =
            "The U I U Squad designated {designation} HasEntered AllRemaining NoSCPsLeft";

        [Description("What should the fakeout cassie text say for UIU")]
        public string FakeoutRespawnAnnouncementsUIUAliveSCPSCassieText { get; set; } =
            "The UIU Squad, designated {designation} has entered the facility. Awaiting recontainment of {scpnum}";

        public string FakeoutRespawnAnnouncementsUIUSCPSDeadCassieText { get; set; } =
            "The UIU Squad, designated {designation} has entered the facility. Substantial threat remains within the facility - exercise caution.";

        [Description("Should the rapid fire tesla gate event work?")]
        public bool RapidFireTelsaEvent { get; set; } = true;

        [Description("How long should this event last for in seconds?")]
        public float RapidFireTeslaEventTiming { get; set; } = 30f;

        [Description("What is the Rapid Fire Tesla Activation Time?")]
        public float RapidFireTeslaEventActivationTime { get; set; } = 0.1f;

        [Description("What is the Rapid Fire Tesla Idle Range?")]
        public float RapidFireTeslaEventIdleRange { get; set; } = 1000f;

        [Description("What is the Rapid Fire Tesla Trigger Range?")]
        public float RapidFireTeslaEventTriggerRange { get; set; } = 500f;

        [Description("What is the Rapid Fire Tesla Cooldown Time?")]
        public float RapidFireTeslaEventCooldownTime { get; set; } = 0.1f;

        [Description("Should players shitting their pants event be active?")]
        public bool PlayerShittingPantsEvent { get; set; } = true;

        [Description("What should the broadcast say to the players when they shit their pants")]
        public string PlayerShittingPantsBroadcast { get; set; } = "Damn it I shit my pants";

        [Description("Should router kicking simulator event be active?")]
        public bool RouterKickingSimulatorEvent { get; set; } = true;

        [Description("How many times should the router be kicked?")]
        public int RouterKickingSimulatorRouterKickAmount { get; set; } = 10;

        [Description("How long should it take in between router kicks?")]
        public float RouterKickingSimulatorTimeBetweenRouterKicks { get; set; } = 10f;

        [Description("How much lag should the players experience from the router kick?")]
        public float RouterKickingSimulatorLagTime { get; set; } = 3f;

        [Description("What should the broadcast say to the player when their router's start to be kicked?")]
        public string RouterKickingSimulatorStartBroadcast { get; set; } =
            "Sorry I had enough with your router for a little bit";

        [Description("What should the broadcast say to the player after the event ends")]
        public string RouterKickingSimulatorEndBroadcast { get; set; } =
            "I kicked your router enough, sorry about that";

        [Description("How long should the delay be **BEFORE** the router kicking simulator starts")]
        public float RouterKickingSimulatorStartWaitTime { get; set; } = 15f;

        [Description(
            "Should the router kicking simulator disable elevators (to avoid people getting teleported into the void")]
        public bool RouterKickingSimulatorDisablesElevators { get; set; } = true;

        [Description("Should events like decontaimination and warhead be disabled for this event")]
        public bool RouterKickingSimulatorDisablesDecomAndNuke { get; set; } = true;

        [Description("Should the super speed event be active?")]
        public bool SuperSpeedEvent { get; set; } = true;

        [Description("How long should the super speed last? (In seconds)")]
        public float SuperSpeedDuration { get; set; } = 15;

        [Description("What should the intensity of the super speed? (Max: 255)")]
        public byte SuperSpeedIntensity { get; set; } = 200;

        [Description("What should the broadcast be that shows to players for super speed")]
        public string SuperSpeedBroadcast { get; set; } = "Gotta go fast";
        
        [Description("Should the drop inventory event be active?")]
        public bool DropInventoryEvent { get; set; } = true;
        
        [Description("What should the broadcast say when a player drops their inventory?")]
        public string DropInventoryText { get; set; } = "You notice a giant hole in your pockets";
        
        [Description("Should the teleport to ally event be enabled?")]
        public bool TeleportToAlliesEvent { get; set; } = true;
        
        [Description("What should the broadcast say when a player is teleported to an ally?")]
        public string TeleportToAllyPlayerTeleportedText { get; set; } = "You have been teleported to an ally";

        [Description("What should the broadcast say when a player gets someone teleported to them?")]
        public string TeleportToAllyNewAllyText { get; set; } = "You had an ally teleported to you";

        [Description("Should the Simon Says event be active?")]
        public bool SimonSaysEvent { get; set; } = true;

        [Description("How long should the time be for the player to do the action?")]
        public float SimonSaysActionTime { get; set; } = 5;

        [Description("Should SCPs be affected bit Simon Says?")]
        public bool SimonSaysAffectsScps { get; set; } = false;
        
        [Description("What should the Simon Says Jump text be?")]
        public string SimonSaysJumpText { get; set; } = "Simon says jump";

        [Description("What should the Someone Says Jump text be?")]
        public string SomeoneSaysJumpText { get; set; } = "Someone says jump";

        [Description("What is the damage punishment if someone doesnt jump if simon says or does jump when someone says")]
        public int SimonSaysDamagePunishment { get; set; } = 50;

        [Description("What should the punishment text be if someone fails the Simon Says event")]
        public string SimonSaysPunishmentText { get; set; } = "You have failed to jump after Simon said too";
        
        [Description("What should the punishment text be if someone fails the Someone Says event")]
        public string SomeoneSaysPunishmentText { get; set; } = "You jumped even though simon didnt say to";
    }
}