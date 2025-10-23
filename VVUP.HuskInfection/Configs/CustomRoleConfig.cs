using System.Collections.Generic;
using VVUP.HuskInfection.Roles;

namespace VVUP.HuskInfection.Configs
{
    public class CustomRoleConfig
    {
        public List<HuskZombie> HuskZombies { get; set; } = new()
        {
            new HuskZombie(),
        };
    }
}