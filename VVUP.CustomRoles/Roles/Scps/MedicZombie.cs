using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.Base;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scps
{
    public class MedicZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 25;
        public override uint Id { get; set; } = 45;
        public override int MaxHealth { get; set; } = 450;
        public override string Name { get; set; } = "<color=#FF0000>Medic SCP-049-2</color>";
        public override string Description { get; set; } = "A zombie that can heal other SCPs";
        public override string CustomInfo { get; set; } = "Medic SCP-049-2";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new EffectEnabler()
            {
                Name = "Move Speed Reduction [Passive]",
                Description = "Slows you down",
                EffectsToApply = new List<ApplyEffects>
                {
                    new()
                    {
                        EffectType = EffectType.Slowness,
                        Intensity = 30,
                    },
                },
            },
            new HealingMist
            {
                Name = "Healing Mist [Active]",
                Description = "Emits an invisible mist that can heal other SCPs",
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
        };
        
        public override string AbilityUsage { get; set; } = "Use your Noclip Button [Left Alt] to swap abilities and to activate. Tap Twice to Swap. Tap Once to activate.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}