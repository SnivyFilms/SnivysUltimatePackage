using System;

namespace VVUP.CustomRoles.API
{
    /// <summary>
    /// Defines the starting team or faction that a player spawns as in SCP: Secret Laboratory.
    /// This enum uses the [Flags] attribute to allow bitwise combinations of multiple team types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use bitwise OR to combine teams (e.g., <c>StartTeam.Scp | StartTeam.Revived</c> for SCP-049-2).
    /// This is particularly useful for roles that belong to multiple factions or have hybrid characteristics.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Assuming a basic Custom Role setup
    /// 
    /// // Single team
    /// StartTeam StartTeam = StartTeam.ClassD;
    /// 
    /// // Combined teams for SCP-049-2 (zombie)
    /// StartTeam StartTeam = StartTeam.Scp | StartTeam.Revived;
    /// 
    /// // Check if a team includes SCP
    /// bool isScp = (team &amp; StartTeam.Scp) != 0;
    /// </code>
    /// </example>
    [Flags]
    public enum StartTeam
    {
        /// <summary>Class-D - For any player who would spawn as a normal Class-D</summary>
        ClassD = 1,
        /// <summary>Scientists - For any player who would spawn as a normal Scientist</summary>
        Scientist = 2,
        /// <summary>Facility Guards - For any player who would spawn as a normal Facility Guard</summary>
        Guard = 4,
        /// <summary>Nine-Tailed Fox - For any player who would spawn as a normal MTF member</summary>
        Ntf = 8,
        /// <summary>Chaos Insurgency - For any player who would spawn as a normal CI member</summary>
        Chaos = 16,
        /// <summary>SCP - For any player who would spawn as any SCP</summary>
        Scp = 32,
        /// <summary>Revived - For any player who would spawn as an SCP-049-2 (Zombie)</summary>
        Revived = 64,
        /// <summary>Escaped players - For any player who escapes through an escape point</summary>
        Escape = 128,
        /// <summary>Other - Basically unused, you can set it for a custom role you don't want to have spawn</summary>
        Other = 256,
        /// <summary>SCP-049 - For any player who would spawn as SCP-049</summary>
        Scp049 = 512,
        /// <summary>SCP-079 - For any player who would spawn as SCP-079</summary>
        Scp079 = 1024,
        /// <summary>SCP-096 - For any player who would spawn as SCP-096</summary>
        Scp096 = 2048,
        /// <summary>SCP-106 - For any player who would spawn as SCP-106</summary>
        Scp106 = 4096,
        /// <summary>SCP-173 - For any player who would spawn as SCP-173</summary>
        Scp173 = 8192,
        /// <summary>SCP-939 - For any player who would spawn as SCP-939</summary>
        Scp939 = 16384,
        /// <summary>SCP-3114 - For any player who would spawn as SCP-3114</summary>
        Scp3114 = 32768,
    }
}