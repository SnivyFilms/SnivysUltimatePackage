using UnityEngine;
using VVUP.Base.EventHandlers;

namespace VVUP.Base.API
{
    /// <summary>
    /// Interface for items that can have a customizable glow effect.
    /// Implement this interface on item classes to enable custom glow rendering with configurable colors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface allows items to override the default glow effect with a custom color.
    /// The glow effect is commonly used for highlighting items in the game world.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// public class MyCustomItem : ICustomItemGlow
    /// {
    ///     public bool HasCustomItemGlow { get; set; } = true;
    ///     public Color CustomItemGlowColor { get; set; } = Color.cyan;
    /// }
    /// </code>
    /// or
    /// <code>
    /// public class MyCustomItem : ICustomItemGlow
    /// {
    ///     public bool HasCustomItemGlow { get; set; } = true;
    ///     public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 191);
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// When configuring through server configs, use <see cref="Color"/> values (normalized 0-1 range).
    /// <see cref="Color32"/> values (0-255 range) are supported in code but will be automatically converted.
    /// </para>
    /// </remarks>
    public interface ICustomItemGlow
    {
        /// <summary>
        /// Gets or sets whether the item should use a custom glow effect.
        /// </summary>
        /// <value>
        /// <c>true</c> if the item should display a custom glow; otherwise, <c>false</c> to disable glow.
        /// </value>
        /// <remarks>
        /// Set this to <c>true</c> and configure <see cref="CustomItemGlowColor"/> to enable the custom item's glow when it's a pickup.
        /// </remarks>
        public bool HasCustomItemGlow { get; set; }
        
        /// <summary>
        /// Gets or sets the color to use for the custom glow effect.
        /// </summary>
        /// <value>
        /// A <see cref="Color"/> value representing the glow color in RGBA format with normalized components (0-1).
        /// </value>
        /// <remarks>
        /// <para>
        /// Accepts both <see cref="Color"/> and <see cref="Color32"/> values in code.
        /// <see cref="Color32"/> values are automatically converted to <see cref="Color"/>.
        /// </para>
        /// <para>
        /// For server configurations, always use <see cref="Color"/> values to avoid null reference errors.
        /// <see cref="Color32"/> may serialize as null in config files.
        /// </para>
        /// </remarks>
        public Color CustomItemGlowColor { get; set; }
        
        /// <summary>
        /// Gets or sets the range (distance) at which the custom glow effect is visible.
        /// </summary>
        /// <value>
        /// A float value representing the glow visibility range in Unity units. Default range varies by implementation.
        /// </value>
        /// <remarks>
        /// <para>
        /// This value determines how far away players can see the item's glow effect.
        /// Higher values make the glow larger, lower values makes the glow smaller.
        /// </para>
        /// </remarks>
        public float GlowRange { get; set; }
        
        /// <summary>
        /// Gets or sets the intensity (brightness) of the custom glow effect.
        /// </summary>
        /// <value>
        /// A float value representing the glow intensity. Default is 1.0f.
        /// </value>
        public float GlowIntensity { get; set; }
        
        /// <summary>
        /// Gets or sets the shadow type for the glow light.
        /// </summary>
        /// <value>
        /// A <see cref="CustomItemEventHandlers.GlowShadowType"/> value. Options are:
        /// <list type="bullet">
        /// <item><c>None</c></item>
        /// <item><c>Hard</c></item>
        /// <item><c>Soft</c></item>
        /// </list>
        /// Default is <c>None</c>.
        /// </value>
        public GlowShadowType ShadowType { get; set; }

        /// <summary>
        /// Gets or sets the position offset for the glow effect relative to the item's center.
        /// </summary>
        /// <value>
        /// A <see cref="Vector3"/> representing the offset in world units
        /// Default is <c>Vector3.zero</c>.
        /// </value>
        /// <remarks>
        /// Use this to adjust the glow position when the item's GameObject center does not align with the visual center of the item.
        /// </remarks>
        public Vector3 GlowOffset { get; set; }
        
        public enum GlowShadowType
        {
            None,
            Hard,
            Soft
        }
    }
}