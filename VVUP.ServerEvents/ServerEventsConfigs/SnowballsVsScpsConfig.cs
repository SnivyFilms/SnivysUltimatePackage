using PlayerRoles;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class SnowballsVsScpsConfig
    {
        public List<RoleTypeId> ScpRoles { get; set; } = new List<RoleTypeId>()
        {
            RoleTypeId.Scp173,
        };

        public List<RoleTypeId> HumanRoles { get; set; } = new List<RoleTypeId>()
        {
            RoleTypeId.Scientist,
            RoleTypeId.FacilityGuard,
            RoleTypeId.NtfCaptain,
            RoleTypeId.NtfPrivate,
            RoleTypeId.NtfSergeant,
            RoleTypeId.NtfSpecialist,
        };

        public float SnowballRefillCycle { get; set; } = 3f;
    }
}