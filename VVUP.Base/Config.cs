using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using UnityEngine;

namespace VVUP.Base
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("There's basically no debug statements for this Module, as its more of the base for everything else.")]
        public bool Debug { get; set; } = false;
        
        [Description("Enable compatibility glow effects for custom items that don't implement ICustomItemGlow")]
        public bool EnableCompatibilityGlow { get; set; } = false;
        
        [Description("You can use this to set custom glow colors for custom items by their Custom Item ID if the plugin they are from doesnt provide a native option. Custom items from VVUP.CI will use a native system and will be ignored here. RGBA is 0-1 (decimals allowed)")]
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
    }

    public class CustomItemGlowConfig
    {
        public uint CustomItemId { get; set; }
        public float R { get; set; } = 1;
        public float G { get; set; } = 1;
        public float B { get; set; } = 1;
        public float A { get; set; } = 1;
        public float GlowRange { get; set; } = 0.25f;
        public float Intensity { get; set; } = 0.25f;
        
        public Color GetColor() => new Color(
            Mathf.Clamp(R, 0f, 1f), 
            Mathf.Clamp(G, 0f, 1f), 
            Mathf.Clamp(B, 0f, 1f), 
            Mathf.Clamp(A, 0f, 1f));
    }
}