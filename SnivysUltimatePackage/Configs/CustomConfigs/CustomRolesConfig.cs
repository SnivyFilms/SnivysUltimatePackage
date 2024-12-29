﻿using System.Collections.Generic;
using System.ComponentModel;
using SnivysUltimatePackage.Custom.Roles;

namespace SnivysUltimatePackage.Configs.CustomConfigs
{
    public class CustomRolesConfig
    {
        [Description("Enables Custom Roles")]
        public bool IsEnabled { get; set; } = true;
        
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
    }
}