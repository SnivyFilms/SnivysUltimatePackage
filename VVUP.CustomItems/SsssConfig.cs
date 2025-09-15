namespace VVUP.CustomItems
{
    public class SsssConfig
    {
        public bool SsssEnabled { get; set; } = true;
        public string Header { get; set; } = "Vicious Vikki's Custom Items.";
        public int DetonateC4Id { get; set; } = 10006;
        public int CustomItemTextId { get; set; } = 1;
        public string DetonateC4Hint { get; set; } = "Press the keybind to activate Detonate C4, you will be able to detonate your C4 (Custom Item).";
        public string DetonateC4SsssText { get; set; } = "Detonate C4";
        public string SsssC4NoC4Deployed { get; set; } = "You haven't placed any C4";
        public string SsssC4DetonatorNeeded { get; set; } = "You need to have your detonator equipped";
        public string SsssC4TooFarAway { get; set; } = "You are far away from your C4, consider getting closer";
        public string SsssDetonateC4ActivationMessage { get; set; } = "Detonating C4";
        public int GrenadeLauncherForceModeId { get; set; } = 10011;
        public string GrenadeLauncherForceModeSsssText { get; set; } = "Toggle ADATS Force";
        public string GrenadeLauncherForceModeHint { get; set; } = "Toggle between full and half force for ADATS";
        public int GrenadeLauncherLaunchModeId { get; set; } = 10012;
        public string GrenadeLauncherLaunchModeSsssText { get; set; } = "Toggle ADATS Impact/Roller";
        public string GrenadeLauncherLaunchModeHint { get; set; } = "Toggle between impact and roller modes for ADATS";
    }
}