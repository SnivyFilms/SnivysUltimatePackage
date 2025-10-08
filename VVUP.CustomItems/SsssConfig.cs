namespace VVUP.CustomItems
{
    public class SsssConfig
    {
        public bool SsssEnabled { get; set; } = true;
        public bool UseHints { get; set; } = true;
        public float TextDisplayDuration { get; set; } = 5f;
        public string Header { get; set; } = "Vicious Vikki's Custom Items.";
        public int DetonateC4Id { get; set; } = 10006;
        public int CustomItemTextId { get; set; } = 1;
        public string DetonateHint { get; set; } = "Press the keybind to activate Detonate C4 or F4, you will be able to detonate your C4/F4.";
        public string DetonateSsssText { get; set; } = "Detonate C4/F4";
        public string SsssNoDeployed { get; set; } = "You haven't placed any C4/F4";
        public string SsssDetonatorNeeded { get; set; } = "You need to have your detonator equipped";
        public string SsssTooFarAway { get; set; } = "You are far away from your C4 or F4, consider getting closer";
        public string SsssDetonateActivationMessage { get; set; } = "Detonating C4/F4";
        public int GrenadeLauncherForceModeId { get; set; } = 10011;
        public string GrenadeLauncherForceModeSsssText { get; set; } = "Toggle ADATS Force";
        public string GrenadeLauncherForceModeHint { get; set; } = "Toggle between full and half force for ADATS";
        public int GrenadeLauncherLaunchModeId { get; set; } = 10012;
        public string GrenadeLauncherLaunchModeSsssText { get; set; } = "Toggle ADATS Impact/Roller";
        public string GrenadeLauncherLaunchModeHint { get; set; } = "Toggle between impact and roller modes for ADATS";
        public int LaserGunRedId { get; set; } = 10020;
        public int LaserGunGreenId { get; set; } = 10021;
        public int LaserGunBlueId { get; set; } = 10022;
        public string LaserGunColorText { get; set; } = "Helios Beam Color";
        public string LaserGunColorTextRed { get; set; } = "(Red)";
        public string LaserGunColorTextGreen { get; set; } = "(Green)";
        public string LaserGunColorTextBlue { get; set; } = "(Blue)";
        public string LaserGunColorHint { get; set; } = "Enter a value between 0 and 255. Any negative number or any number above 255 will be random";
    }
}