using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace VVUP.ScpChanges
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        public string Scp1576Text { get; set; } = "<size=24><align=left>Spectators: %spectators%. Time before next spawn wave: %timebeforespawnwave% seconds</align></size>";
        public float Scp1576TextDuration { get; set; } = 15f;
        [Description("Add %customroles% to the Scp1576Text to show custom roles when used.")]
        public Dictionary<uint, string> Scp1576CustomRolesAlive { get; set; } = new()
        {
            { 25, "<color=#ff00ff><size=30>Serpents Hand Guardian</color></size>" },
            { 26, "<color=#ff00ff><size=30>Serpents Hand Enchanter</color></size>" },
            { 27, "<color=#ff00ff><size=30>Serpents Hand Agent</color></size>" },
        };
        [Description("Add %roles% to the Scp1576Text to show alive roles when used.")]
        public Dictionary<RoleTypeId, string>AliveRoles { get; set; } = new()
        {
            { RoleTypeId.Scp049, "<color=#ff0000><size=30>SCP-049</color></size>" },
            { RoleTypeId.Scp0492, "<color=#ff0000><size=30>SCP-049-2</color></size>" },
            { RoleTypeId.Scp096, "<color=#ff0000><size=30>SCP-096</color></size>" },
            { RoleTypeId.Scp173, "<color=#ff0000><size=30>SCP-173</color></size>" },
            { RoleTypeId.Scp106, "<color=#ff0000><size=30>SCP-106</color></size>" },
            { RoleTypeId.Scp939, "<color=#ff0000><size=30>SCP-939</size></color></size>" },
        };
        [Description("Add %teams% to the Scp1576Text to show alive teams when used.")]
        public Dictionary<Team, string> AliveTeams { get; set; } = new()
        {
            { Team.FoundationForces, "<size=30>MTF</size>" },
            { Team.ChaosInsurgency, "<size=30>Chaos Insurgency</size>" },
            { Team.Scientists, "<size=30>Scientists</size>" },
            { Team.ClassD, "<size=30>D-Class</size>" },
            { Team.SCPs, "<size=30>SCPs</size>" },
        };
        /*
        [Description("Does SCP-096 get unlimited rage when the Nuke is detonating?")]
        public bool Scp096UnlimitedRageDuringNuke { get; set; } = true;
        */

        public List<ScpHealth> ScpHealths { get; set; } = new()
        {
            new ScpHealth()
            {
                Role = RoleTypeId.Scp106,
                MaxHealth = 600,
                HumeShield = 500,
                ApplyToCustomRoles = false,
            },
        };

        public List<ScpDamageResistance> ScpDamageResistances { get; set; } = new()
        {
            new ScpDamageResistance()
            {
                Role = RoleTypeId.Scp106,
                DamageType = DamageType.Firearm,
                ResistanceModifier = 0.1f,
                ShouldResistWithHume = false,
                ShouldApplyToCustomRoles = false,
            }
        };
        [Description("Scp Set Damages allow you to set specific damage values for specific roles and damage types, this will override any other damage modifications. ShouldOneShot will do what would normally need multiple hits to apply instantly, such as a player being sent to the 106 pocket dimention")]
        public List<ScpSetDamage> ScpSetDamages { get; set; } = new()
        {
            new ScpSetDamage()
            {
                Role = RoleTypeId.Scp106,
                Damage = 12f,
                ShouldOneShotApplyEffects = true,
                ShouldApplyToCustomRoles = false,
            },
            new ScpSetDamage()
            {
                Role = RoleTypeId.Scp049,
                Damage = 9999,
                ShouldOneShotApplyEffects = false,
            }
        };
    }

    public class ScpHealth
    {
        public RoleTypeId Role { get; set; }
        public int MaxHealth { get; set; }
        public int HumeShield { get; set; }
        public float HumeShieldRegenMultiplier { get; set; }
        public bool ApplyToCustomRoles { get; set; } = false;
    }

    public class ScpDamageResistance
    {
        public RoleTypeId Role { get; set; }
        public float ResistanceModifier { get; set; }
        public DamageType DamageType { get; set; }
        public bool ShouldResistWithHume { get; set; } = false;
        public bool ShouldApplyToCustomRoles { get; set; } = false;
    }
    
    public class ScpSetDamage
    {
        public RoleTypeId Role { get; set; }
        public float Damage { get; set; }
        public bool ShouldOneShotApplyEffects { get; set; } = false;
        public Dictionary<RoleTypeId, float> DamageMultipliersAgainstSpecificRoles { get; set; }
        public bool ShouldApplyToCustomRoles { get; set; } = false;
    }
}