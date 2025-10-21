using UnityEngine;

namespace VVUP.CustomItems.API
{
    public interface ICustomItemGlow
    {
        public bool HasCustomItemGlow { get; set; }
        public Color CustomItemGlowColor { get; set; }
    }
}