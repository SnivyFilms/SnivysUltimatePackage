﻿using System.Collections.Generic;
using System.ComponentModel;
using SnivysUltimatePackage.Custom.Roles.Chaos;
using SnivysUltimatePackage.Custom.Roles.ClassD;
using SnivysUltimatePackage.Custom.Roles.Foundation;
using SnivysUltimatePackage.Custom.Roles.OpenCustomRoles;
using SnivysUltimatePackage.Custom.Roles.Other;
using SnivysUltimatePackage.Custom.Roles.Scientist;
using SnivysUltimatePackage.Custom.Roles.Scps;

namespace SnivysUltimatePackage.Configs.CustomConfigs
{
    public class CustomRolesConfig
    {
        [Description("Enables Custom Roles")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enables the 5 free custom roles")]
        public bool EnableFreeCustomRoles { get; set; } = false;
        
        public List<ContainmentScientist> ContainmentScientists { get; set; } = new()
        {
            new ContainmentScientist(),
        };
        public List<LightGuard> LightGuards { get; set; } = new()
        {
            new LightGuard(),
        };
        public List<Biochemist> Biochemists { get; set; } = new()
        {
            new Biochemist(),
        };
        public List<ContainmentGuard> ContainmentGuards { get; set; } = new()
        {
            new ContainmentGuard(),
        };
        public List<BorderPatrol> BorderPatrols { get; set; } = new()
        {
            new BorderPatrol(),
        };
        public List<Nightfall> Nightfalls { get; set; } = new()
        {
            new Nightfall(),
        };
        public List<A7Chaos> A7Chaoss { get; set; } = new()
        {
            new A7Chaos(),
        };
        public List<Flipped> Flippeds { get; set; } = new()
        {
            new Flipped(),
        };
        public List<TelepathicChaos> TelepathicChaos { get; set; } = new()
        {
            new TelepathicChaos(),
        };
        public List<JuggernautChaos> JuggernautChaos { get; set; } = new()
        {
            new JuggernautChaos(),
        };
        public List<CISpy> CISpies { get; set; } = new()
        {
            new CISpy(),
        };
        public List<MtfWisp> MtfWisps { get; set; } = new()
        {
            new MtfWisp(),
        };
        public List<DwarfZombie> DwarfZombies { get; set; } = new()
        {
            new DwarfZombie(),
        };
        public List<ExplosiveZombie> ExplosiveZombies { get; set; } = new()
        {
            new ExplosiveZombie(),
        };
        public List<CiPhantom> CiPhantoms { get; set; } = new()
        {
            new CiPhantom(),
        };

        public List<MedicZombie> MedicZombies { get; set; } = new()
        {
            new MedicZombie(),
        };

        public List<LockpickingClassD> LockpickingClassDs { get; set; } = new()
        {
            new LockpickingClassD(),
        };

        public List<Demolitionist> Demolitionists { get; set; } = new()
        {
            new Demolitionist(),
        };
        
        public List<Vanguard> Vanguards { get; set; } = new()
        {
            new Vanguard(),
        };

        public List<TheoreticalPhysicistScientist> TheoreticalPhysicistScientists { get; set; } = new()
        {
            new TheoreticalPhysicistScientist(),
        };
        
        [Description("These custom roles are for the server owner to make some custom roles themselves, there will be no support for any custom roles that you make with these")]
        public List<FreeCustomRole1> FreeCustomRoles1 { get; set; } = new()
        {
            new FreeCustomRole1(),
        };
        public List<FreeCustomRole2> FreeCustomRoles2 { get; set; } = new()
        {
            new FreeCustomRole2(),
        };
        public List<FreeCustomRole3> FreeCustomRoles3 { get; set; } = new()
        {
            new FreeCustomRole3(),
        };
        public List<FreeCustomRole4> FreeCustomRoles4 { get; set; } = new()
        {
            new FreeCustomRole4(),
        };
        public List<FreeCustomRole5> FreeCustomRoles5 { get; set; } = new()
        {
            new FreeCustomRole5(),
        };
    }
}