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
    public class CustomItemsConfig
    {
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
        
        public List<LowGravityGrenade> LowGravityGrenades { get; private set; } = new()
        {
            new LowGravityGrenade()
        };
    }
}