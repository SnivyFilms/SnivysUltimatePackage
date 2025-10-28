using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scps
{
    public class SoundBreaker173 : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 10;
        public override uint Id { get; set; } = 58;
        public override int MaxHealth { get; set; } = 3800;
        public override string Name { get; set; } = "<color=#FF0000>Sound Breaker 173</color>";
        public override string Description { get; set; } = "Reset blink cooldown, reduce the next Blink interval and range";
        public override string CustomInfo { get; set; } = "Sound Breaker SCP-173";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp173;

        public StartTeam StartTeam { get; set; } = StartTeam.Scp173;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new SoundBreaker()
            {
                Name = "Sound Breaker [Active]",
                Description = "Reset blink cooldown, reduce the next Blink interval and range",
            }
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.Scp173,
                    Chance = 100,
                },
            },
        };

        public override string AbilityUsage { get; set; } = "Use your Sound Breaker ability with your server keybind (set it in ESC > Settings > Server Settings).";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}