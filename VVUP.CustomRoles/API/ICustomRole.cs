namespace VVUP.CustomRoles.API
{
    /// <summary>
    /// Interface defining spawn properties for a custom role in the game.
    /// Implement this interface to create custom roles with configurable team assignments and spawn probabilities.
    /// </summary>
    /// <remarks>
    /// Custom roles extend the base game functionality by allowing developers to define
    /// unique roles with specific team affiliations and spawn chances. These roles are
    /// assigned to players at the start of each game based on their configured probability.
    /// </remarks>
    public interface ICustomRole
    {
        /// <summary>
        /// Gets or sets the team this role is assigned to at the start of a game.
        /// </summary>
        /// <value>
        /// A <see cref="StartTeam"/> value representing the initial team assignment.
        /// See <see cref="StartTeam"/> for available team options.
        /// </value>
        /// <remarks>
        /// This property determines which team the player will join when they spawn with this role.
        /// Team assignment doesn't mean that the player needs to be that team.
        /// <para>
        /// Spawn Timing:
        /// <list type="bullet">
        /// <item><description>Class-D, Scientists, Facility Guards, and non-zombie SCPs spawn at the beginning of the round.</description></item>
        /// <item><description>NTF (Nine-Tailed Fox) and Chaos Insurgency spawn during respawn waves.</description></item>
        /// <item><description>Revived roles spawn when an SCP-049 revives a dead player.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// If you want to have a custom role that has a start team of Class-D for a Class-D Custom Role, you can do:
        /// <code>
        /// StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        /// </code>
        /// If you want a custom role like a Chaos Insurgent that takes a place of a Facility Guard, you can do:
        /// <code>
        /// StartTeam StartTeam { get; set; } = StartTeam.FacilityGuard;
        /// </code>
        /// </example>
        public StartTeam StartTeam { get; set; }
        
        /// <summary>
        /// Gets or sets the probability of this role being assigned to a player.
        /// </summary>
        /// <value>
        /// An integer value between 0 and 100 representing the percentage chance of assignment.
        /// A value of 0 means the role will never spawn, while 100 guarantees assignment if possible.
        /// </value>
        /// <remarks>
        /// The spawn chance is evaluated each time a player spawns, given the <see cref="StartTeam"/> for the custom role matches. 
        /// </remarks>
        public int Chance { get; set; }
    }
}