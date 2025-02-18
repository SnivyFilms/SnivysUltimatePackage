﻿using System.Collections.Generic;
using System.ComponentModel;
using SnivysUltimatePackageOneConfig.Custom.Items.Armor;
using SnivysUltimatePackageOneConfig.Custom.Items.Firearms;
using SnivysUltimatePackageOneConfig.Custom.Items.Grenades;
using SnivysUltimatePackageOneConfig.Custom.Items.MedicalItems;
//using SnivysUltimatePackageOneConfig.Custom.Items.Keycards;
using SnivysUltimatePackageOneConfig.Custom.Items.Other;

namespace SnivysUltimatePackageOneConfig.Configs.CustomConfigs
{
    public class CustomItemsConfig
    {
        [Description("Enables Custom Items")]
        public bool IsEnabled { get; set; } = true;
        
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

        /*public List<MediGun> MediGuns { get; private set; } = new()
        {
            new MediGun()
        };*/

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

        /*public List<Scp2818> Scp2818s { get; private set; } = new()
        {
            new Scp2818()
        };*/

        public List<InfinitePills> InfinitePills { get; private set; } = new()
        {
            new InfinitePills()
        };
        
        //public List<ContainmentScientistKeycard> ContainmentScientistKeycards { get; private set; } = new()
        //{
        //    new ContainmentScientistKeycard()
        //};

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
    }
}