namespace VVUP.CustomRoles
{
    public class SsssConfig
    {
        public bool SsssEnabled { get; set; } = true;
        public string Header { get; set; } = "Vicious Vikki's Custom Roles";
        public int ActiveCamoId { get; set; } = 10101;
        public int ChargeId { get; set; } = 10102;
        public int DetectId { get; set; } = 10103;
        public int DoorPickingId { get; set; } = 10104;
        public int HealingMistId { get; set; } = 10105;
        public int RemoveDisguiseId { get; set; } = 10106;
        //public int ReviveMistId { get; set; } = 10107;
        public int TeleportId { get; set; } = 10108;
        public int SoundBreakerId { get; set; } = 10109;
        public int ReplicatorId { get; set; } = 10110;
        public int CustomRoleTextId { get; set; } = 2;
        public string ActiveCamoHint { get; set; } = "Press the keybind to activate Active Camo, you will become invisible for a short time (Custom Ability).";
        public string ChargeHint { get; set; } = "Press the keybind to activate Charge, you will be able to charge at a target (Custom Ability).";
        public string DetectHint { get; set; } = "Press the keybind to activate Detect, you will be able to detect nearby players (Custom Ability).";
        public string DoorPickingHint { get; set; } = "Press the keybind to activate Door Picking, you will be able to pick doors (Custom Ability).";
        public string HealingMistHint { get; set; } = "Press the keybind to activate Healing Mist, you will be able to heal yourself and nearby players (Custom Ability).";
        public string RemoveDisguiseHint { get; set; } = "Press the keybind to activate Remove Disguise, you will be able to remove your disguise (Custom Ability).";
        //public string ReviveMistHint { get; set; } = "Press the keybind to activate Revive Mist, you will be able to revive nearby players (Custom Ability).";
        public string TeleportHint { get; set; } = "Press the keybind to activate Teleport, you will be able to teleport to a target location (Custom Ability).";
        public string SoundBreakerHint { get; set; } = "Press the keybind to reset Blink cooldown and reduce the next blink interval and distance (Custom Ability).";
        public string ReplicatorHint { get; set; } = "Press the keybind to create Decoy and make safe Recon (Custom Ability).";
        public string ActiveCamoSsssText { get; set; } = "Active Camo";
        public string ChargeSsssText { get; set; } = "Charge";
        public string DetectSsssText { get; set; } = "Detect";
        public string DoorPickingSsssText { get; set; } = "Door Picking";
        public string HealingMistSsssText { get; set; } = "Healing Mist";
        public string RemoveDisguiseSsssText { get; set; } = "Remove Disguise";
        //public string ReviveMistSsssText { get; set; } = "Reviving Mist";
        public string TeleportSsssText { get; set; } = "Teleport";
        public string SoundBreakerSsssText { get; set; } = "Sound Breaker";
        public string ReplicatorSsssText { get; set; } = "Replicator";
        public string SsssActiveCamoActivationMessage { get; set; } = "Activated Active Camo";
        public string SsssChargeActivationMessage { get; set; } = "Activated Charge";
        public string SsssDoorPickingActivationMessage { get; set; } = "Activated Door Picking, Interact with the door you want to pick.";
        public string SsssHealingMistActivationMessage { get; set; } = "Activated Healing Mist";
        public string SsssRemoveDisguiseActivationMessage { get; set; } = "Removing Disguise";
        //public string SsssReviveMistActivationMessage { get; set; } = "Activated Revive Mist";
        public string SsssTeleportActivationMessage { get; set; } = "Activated Teleport";
        public string SsssSoundBreakerActivationMessage { get; set; } = "Activated Sound Breaker";
        public string SsssReplicatorActivationMessage { get; set; } = "Activated Replicator";
        public int RoundStartRolesId { get; set; } = 10111;
        public int RespawnWaveRolesId { get; set; } = 10112;
        public int Scp049ReviveRolesId { get; set; } = 10113;
        public string RoundStartRolesSsssText { get; set; } = "Enable getting Custom Roles on Round Start";
        public string RespawnWaveRolesSsssText { get; set; } = "Enable getting Custom Roles on Respawn Waves";
        public string Scp049ReviveRolesSsssText { get; set; } = "Enable getting Custom Roles on SCP-049-2 Revivals";
        public string CustomRoleReceivingEnabledText { get; set; } = "True";
        public string CustomRoleReceivingDisabledText { get; set; } = "False";
    }
}