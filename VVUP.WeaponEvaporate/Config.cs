using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;

namespace VVUP.WeaponEvaporate
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("Left is the Damage Type, such as damage coming from a Revolver, or a MicroHID. The Right is the HitboxType required to cause evaporation, Types are 'Body', 'Headshot', 'Limb', and 'Any' for HitboxTypes. Any should be used for non-standard damage types like the MicroHID.")]
        public Dictionary<DamageType, EventHandlers.HitBoxEnums> WeaponHitToEvaporate { get; set; } = new Dictionary<DamageType, EventHandlers.HitBoxEnums>
        {
            { DamageType.MicroHid, EventHandlers.HitBoxEnums.Any },
            { DamageType.Revolver, EventHandlers.HitBoxEnums.Headshot },
        };
    }
}