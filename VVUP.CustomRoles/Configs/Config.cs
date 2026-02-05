using System.ComponentModel;
using Exiled.API.Interfaces;

namespace VVUP.CustomRoles.Configs
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Delay in seconds before registering custom roles spawn conditions when the server starts. This can cover pretty much all of the custom roles that uses this modules API for spawning conditions. You can set it to 0 if you want them to register instantly.")]
        public float DelayBeforeRegisteringCustomRolesSpawnConditions { get; set; } = 2f;
        public CustomRolesConfig CustomRolesConfig { get; set; } = new();
        //public CustomRoleWrapperConfig CustomRoleWrapperConfig { get; set; } = new();
        public SsssConfig SsssConfig { get; set; } = new();
    }
}