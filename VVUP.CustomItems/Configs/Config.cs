using System.ComponentModel;
using Exiled.API.Interfaces;

namespace VVUP.CustomItems.Configs
{
    public class Config : IConfig
    {
        [Description("Enables Custom Items")]
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;
        public CustomItemsConfig CustomItemsConfig { get; set; } = new();
        public SsssConfig SsssConfig { get; set; } = new();
    }
}