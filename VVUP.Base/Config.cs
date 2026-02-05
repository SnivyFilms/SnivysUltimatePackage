using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using UnityEngine;
using VVUP.Base.API;
using VVUP.Base.EventHandlers;

namespace VVUP.Base
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("There's basically no debug statements for this Module, as its more of the base for everything else.")]
        public bool Debug { get; set; } = false;
        
        [Description("Enable compatibility glow effects for custom items that don't implement ICustomItemGlow")]
        public bool EnableCompatibilityGlow { get; set; } = false;
        
        [Description("You can use this to set custom glow colors for custom items by their Custom Item ID if the plugin they are from doesnt provide a native option. Custom items from VVUP.CI will use a native system and will be ignored here.")]
        public List<CustomItemGlowConfig> CustomItemGlow { get; set; } = new List<CustomItemGlowConfig>
        {
            new CustomItemGlowConfig
            {
                CustomItemId = 0,
                R = 1,
                G = 0, 
                B = 0,
                A = 0,
                GlowRange = 0.25f,
                Intensity = 1f,
            },
        };
        [Description("Some people may have errors with items getting glows at round start if they start on the ground, this delay should hopefully mitigate that.")]
        public float DelayBeforeColorsAppliesToItemsAlreadyOnGroundAtRoundStart { get; set; } = 1f;
    }

    public class CustomItemGlowConfig
    {
        public uint CustomItemId { get; set; }
        [Description("RGBA values should be between 0 and 1.")]
        public float R { get; set; } = 1;
        public float G { get; set; } = 1;
        public float B { get; set; } = 1;
        public float A { get; set; } = 1;
        [Description("Glow range can be any value, but I recommend it to be between 0 and 1 in most cases.")]
        public float GlowRange { get; set; } = 0.25f;
        [Description("Intensity should be between 0 and 1.")]
        public float Intensity { get; set; } = 0.25f;
        [Description("The options are None, Soft, and Hard for shadow types.")]
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        [Description("Offsets can be used to adjust the position of the glow. Useful for items that have their 'center' not in the center of the item, such as the E-11.")]
        public float OffsetX { get; set; } = 0f;
        public float OffsetY { get; set; } = 0f;
        public float OffsetZ { get; set; } = 0f;
    
        public Color GetColor() => new Color(
            Mathf.Clamp(R, 0f, 1f), 
            Mathf.Clamp(G, 0f, 1f), 
            Mathf.Clamp(B, 0f, 1f), 
            Mathf.Clamp(A, 0f, 1f));
    
        public Vector3 GetOffset() => new Vector3(OffsetX, OffsetY, OffsetZ);
    }
}