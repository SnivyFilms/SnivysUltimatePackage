using System.Collections.Generic;
using VVUP.HuskInfection.Items;

namespace VVUP.HuskInfection.Configs
{
    public class CustomItemConfig
    {
        public List<HuskGrenade> HuskGrenades { get; set; } = new()
        {
            new HuskGrenade(),
        };

        public List<Calyxanide> Calyxanides { get; set; } = new()
        {
            new Calyxanide(),
        };
    }
}