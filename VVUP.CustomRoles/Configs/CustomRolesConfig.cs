using System.Collections.Generic;
using VVUP.CustomRoles.Roles.Chaos;
using VVUP.CustomRoles.Roles.ClassD;
using VVUP.CustomRoles.Roles.Foundation;
using VVUP.CustomRoles.Roles.Other;
using VVUP.CustomRoles.Roles.Scientist;
using VVUP.CustomRoles.Roles.Scps;

namespace VVUP.CustomRoles.Configs
{
    public class CustomRolesConfig
    {
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
        
        public List<MtfParamedic> MtfParamedics { get; set; } = new()
        {
            new MtfParamedic(),
        };
        
        public List<ClassDAnalyst> ClassDAnalysts { get; set; } = new()
        {
            new ClassDAnalyst(),
        };
        
        public List<ClassDTank> ClassDTanks { get; set; } = new()
        {
            new ClassDTank(),
        };
        
        public List<InfectedZombie> InfectedZombies { get; set; } = new()
        {
            new InfectedZombie(),
        };
        
        public List<PoisonousZombie> PoisonousZombies { get; set; } = new()
        {
            new PoisonousZombie(),
        };

        public List<SpeedsterZombie> SpeedsterZombies { get; set; } = new()
        {
            new SpeedsterZombie(),
        };

        public List<TeleportZombie> TeleportZombies { get; set; } = new()
        {
            new TeleportZombie()
        };

        public List<SoundBreaker173> SoundBreaker173 { get; set; } = new()
        {
            new SoundBreaker173()
        };

        public List<Replicant> Replicant { get; set; } = new()
        {
            new Replicant()
        };
        
        public List<Dwarf> Dwarfs { get; set; } = new()
        {
            new Dwarf()
        };
        
        public List<PaperMan> PaperMen { get; set; } = new()
        {
            new PaperMan()
        };
        
        public List<QuantumGravityResearcher> QuantumGravityResearchers { get; set; } = new()
        {
            new QuantumGravityResearcher()
        };
    }
}