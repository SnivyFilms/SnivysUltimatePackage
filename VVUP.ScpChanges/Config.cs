using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("Old SCP 106 Behavior means that SCP 106 will only have lower health, but a damage resistance to bullets.")]
        public bool OldScp106Behavior { get; set; } = true;
        public int Scp106Health { get; set; } = 600;
        [Description("0.1 = 90% resistance, 0.2 = 80% resistance, etc.")]
        public float Scp106DamageResistance { get; set; } = 0.1f;
        [Description("If SCP-106 has a damage resistance, should its Hume Shield be affected by the resistance as well")]
        public bool ResistanceWithHume { get; set; } = false;
        [Description("Can SCP-106 one shot people to the pocket dimension? This is separate from the old behavior setting.")]
        public bool Scp106OneShot { get; set; } = true;
        [Description("Can SCP-049 one shot people?")]
        public bool Scp049OneShot { get; set; } = true;
    }
}