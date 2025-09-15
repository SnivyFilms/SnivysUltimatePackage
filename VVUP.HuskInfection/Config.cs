using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using VVUP.CustomRoles.Roles.Scps;

namespace VVUP.HuskInfection
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public CustomItemConfig CustomItemConfig { get; set; } = new();
        public CustomRoleConfig CustomRoleConfig { get; set; } = new();
        [Description("SSSS Stuff")]
        public bool SsssEnabled { get; set; } = true;
        public string Header { get; set; } = "Vicious Vikki's Husk Infection";
        public int HuskInfectionTextId { get; set; } = 4;
    }
}