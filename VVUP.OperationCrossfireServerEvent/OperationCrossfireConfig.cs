using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using VVUP.OperationCrossfireServerEvent.Items;

namespace VVUP.OperationCrossfireServerEvent
{
    public class OperationCrossfireConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string StartEventCassieMessage { get; set; } = "Military Sim Started";
        public string StartEventCassieText { get; set; } = "\nMilitary Simulation (Operation Crossfire)\nReference the objectives at the top of the screen.";
        
        [Description("How often should the event check for updates? (in Seconds)")]
        public float CheckForEventsInterval { get; set; } = 1f;
        
        [Description("The ratios of MTF & Scientist that spawns during the event. D-Class will be whatever is left. Must add up to 1")]
        public float MtfRatio { get; set; } = 0.5f;
        public float ScientistRatio { get; set; } = 0.25f;

        [Description("What should the broadcast say for someone who connects during the event?")]
        public string PlayerConnectDuringEventMessage { get; set; } =
            "There's an active event going on, please hang tight.";
        [Description("How long should this be displayed to the user?")]
        public float PlayerConnectDuringEventMessageDisplayDuration { get; set; } = 10f;

        public uint PrototypeKeycardBasicId { get; set; } = 44;

        public float ScientistPercentageRequiredToWin { get; set; } = 0.5f;
        
        public float MtfPercentageRequiredToWin { get; set; } = 0.2f;

        public string MtfScientistWinMessage { get; set; } =
            "MTF and Scientists has successfully completed their objectives";

        public string ClassDWinMessage { get; set; } =
            "Class-D has successfully caused enough disturbance to the MTF and Scientists";
        public string TieMessage { get; set; } = "Both sides has been unable to complete their objectives";

        [Description("How long should the victory message be displayed for a victory condition?")]
        public int EndOfRoundTime { get; set; } = 10;
        [Description("How long should the event last? (This uses the nuke timer)")]
        public int EventDuration { get; set; } = 1800;

        public string MtfScientistObjective1 { get; set; } = "Find and refine prototype keycard";
        public string MtfScientistObjective2 { get; set; } = "Unlock SCP-914";
        public string MtfObjective3 { get; set; } = "Escort Scientists out of the facility";
        public string ScientistObjective3 { get; set; } = "Escape the facility";
        public string ClassDObjective1 { get; set; } = "Find and hold the prototype keycard";
        public string ClassDObjective2 { get; set; } = "Kill MTF, Hold Scientists as Hostages";

        public int StartingBroadcastTime { get; set; } = 60;
        
        [Description("What firearms should Class-D spawn with? (Randomly Picks one)")]
        public List<ItemType> ClassDFirearms { get; set; } = new List<ItemType>
        {
            ItemType.GunAK,
            ItemType.GunShotgun,
            ItemType.GunRevolver,
            ItemType.GunLogicer,
        };
        [Description("What Keycard should Class-D spawn with?")]
        public ItemType ClassDKeycard { get; set; } = ItemType.KeycardMTFPrivate;

        [Description("What should it say if Class-D is unable to harm unarmed Scientists")]
        public string ClassDScientistHostagesUnableToHarm { get; set; } =
            "The Scientists are your hostages as of now, best to keep them alive";
        [Description("What should it say if Class-D is able to harm unarmed Scientists")]
        public string ClassDScientistsNowAreTargets { get; set; } =
            "The Scientists are now vulnerable, now kill them";
        
        public List<OcfPrototypeKeycardBasic> OcfPrototypeKeycardBasics { get; private set; } = new()
        {
            new OcfPrototypeKeycardBasic()
        };

        public List<OcfPrototypeKeycardRefined> OcfPrototypeKeycardRefineds { get; private set; } = new()
        {
            new OcfPrototypeKeycardRefined()
        };
    }
}