using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using VVUP.CustomItems.Items.Armor;
using VVUP.CustomItems.Items.Firearms;
using VVUP.CustomItems.Items.Grenades;
//using VVUP.CustomItems.Items.Keycards;
using VVUP.CustomItems.Items.MedicalItems;
using VVUP.CustomItems.Items.Other;

namespace VVUP.CustomItems
{
    public class CustomItemsConfig : IConfig
    {
        [Description("Enables Custom Items")]
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        public List<SmokeGrenade> SmokeGrenades { get; private set; } = new()
        {
            new SmokeGrenade()
        };

        public List<ExplosiveRoundRevolver> ExplosiveRoundRevolvers { get; private set; } = new()
        {
            new ExplosiveRoundRevolver()
        };
        
        public List<NerveAgentGrenade> NerveAgentGrenades { get; set; } = new()
        {
            new NerveAgentGrenade()
        };

        public List<DeadringerSyringe> DeadringerSyringes { get; private set; } = new()
        {
            new DeadringerSyringe()
        };

        public List<PhantomLantern> PhantomLanterns { get; private set; } = new()
        {
            new PhantomLantern()
        };

        public List<ExplosiveResistantArmor> ExplosiveResistantArmor { get; private set; } = new()
        {
            new ExplosiveResistantArmor()
        };

        public List<KySyringe> KySyringes { get; private set; } = new()
        {
            new KySyringe()
        };

        public List<MediGun> MediGuns { get; private set; } = new()
        {
            new MediGun()
        };

        public List<Tranquilizer> Tranquilizers { get; private set; } = new()
        {
            new Tranquilizer()
        };

        public List<Scp1499> Scp1499s { get; private set; } = new()
        {
            new Scp1499()
        };

        public List<EmpGrenade> EmpGrenades { get; private set; } = new()
        {
            new EmpGrenade()
        };

        public List<AntiScp096Pills> AntiScp096Pills { get; private set; } = new()
        {
            new AntiScp096Pills()
        };

        public List<C4> C4s { get; private set; } = new()
        {
            new C4()
        };

        public List<Scp2818> Scp2818s { get; private set; } = new()
        {
            new Scp2818()
        };

        public List<InfinitePills> InfinitePills { get; private set; } = new()
        {
            new InfinitePills()
        };
        
        /*public List<ContainmentScientistKeycard> ContainmentScientistKeycards { get; private set; } = new()
        {
            new ContainmentScientistKeycard()
        };*/

        public List<ClusterGrenade> ClusterGrenades { get; private set; } = new()
        {
            new ClusterGrenade()
        };
        
        public List<AdditionalHealth207> AdditionalHealth207s { get; private set; } = new()
        {
            new AdditionalHealth207()
        };
        
        public List<LowGravityArmor> LowGravityArmors { get; private set; } = new()
        {
            new LowGravityArmor()
        };

        public List<ViperPdw> ViperPdws { get; private set; } = new()
        {
            new ViperPdw()
        };

        public List<Pathfinder> Pathfinders { get; private set; } = new()
        {
            new Pathfinder()
        };
        
        public List<LaserGun> LaserGuns { get; private set; } = new()
        {
            new LaserGun()
        };

        public List<MultiFlash> MultiFlashes { get; private set; } = new()
        {
            new MultiFlash()
        };

        public List<ProxyBang> ProxyBangs { get; private set; } = new()
        {
            new ProxyBang()
        };
        
        public List<GrenadeLauncher> GrenadeLaunchers { get; private set; } = new()
        {
            new GrenadeLauncher()
        };

        public List<PortableIntercom> PortableIntercoms { get; private set; } = new()
        {
            new PortableIntercom()
        };

        public List<Telewand> Telewand { get; private set; } = new()
        {
            new Telewand()
        };

        [Description("SSSS Stuff")]
        public string Header { get; set; } = "VVUP Custom Items.";
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