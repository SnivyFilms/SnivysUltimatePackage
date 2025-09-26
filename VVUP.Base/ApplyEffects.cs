using Exiled.API.Enums;

namespace VVUP.Base
{
    public class ApplyEffects
    {
        public EffectType EffectType { get; set; }
        public byte Intensity { get; set; } = 1;
        public float Duration { get; set; } = 0;
        public bool AddDurationIfActive { get; set; } = false;
    }
}