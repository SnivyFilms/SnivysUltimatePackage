/*using System.Collections.Generic;
using System.ComponentModel;
using Exiled.CustomRoles.API.Features;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Configs
{
    public class CustomRoleWrapperConfig
    {
        [Description("Enables custom roles that dont use this plugin's API to be compatible with this plugin's role spawning system")]
        public bool EnableCustomRoleCompatibility { get; set; } = false;
        [Description("A list of custom roles to be registered with their start team and chance to spawn, VVUP.CR, VVUP.FCR, and VVUP.HK all natively support this system and doesnt need to be defined again, as it will do nothing if you define them again.")]
        public List<CustomRoleWrapper> CustomRoles { get; set; } = new List<CustomRoleWrapper>
        {
            new CustomRoleWrapper
            {
                CustomRoleId = 0,
                StartTeam = StartTeam.Other,
                Chance = 0,
                Limit = 0,
            },
        };
    }

    public class CustomRoleWrapper
    {
        public uint CustomRoleId { get; set; } = 0;
        public StartTeam StartTeam { get; set; } = StartTeam.Other;
        public int Chance { get; set; } = 0;
        public int Limit { get; set; } = 0;
    }
    
    public class CustomRoleWrapperAdapter : ICustomRole
    {
        private readonly CustomRoleWrapper _wrapper;
        private CustomRole _cachedRole;
        
        public CustomRoleWrapperAdapter(CustomRoleWrapper wrapper)
        {
            _wrapper = wrapper;
            StartTeam = wrapper.StartTeam;
            Chance = wrapper.Chance;
        }

        public StartTeam StartTeam { get; set; }
        public int Chance { get; set; }
        
        public static implicit operator CustomRole(CustomRoleWrapperAdapter adapter)
        {
            return adapter._cachedRole ??= CustomRole.Get(adapter._wrapper.CustomRoleId);
        }
    }
}*/