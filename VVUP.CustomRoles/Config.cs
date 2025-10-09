using Exiled.API.Interfaces;

namespace VVUP.CustomRoles
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public CustomRolesConfig CustomRolesConfig { get; set; } = new();
        public SsssConfig SsssConfig { get; set; } = new();
    }
}