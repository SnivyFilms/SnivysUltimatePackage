using UnityEngine;

namespace VVUP.Base.API
{
    public interface ICustomItemGlow
    {
        public bool HasCustomItemGlow { get; set; }
        public Color CustomItemGlowColor { get; set; }
    }
}