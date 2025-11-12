using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.WeaponEvaporate
{
    public class EventHandlers
    {
        public Plugin Plugin;
        public EventHandlers(Plugin plugin) => Plugin = plugin;
        public enum HitBoxEnums
        {
            Body, Headshot, Limb, Any
        }
        
        public void OnDying(DyingEventArgs ev)
        {
            if (ev.Player == null)
                return;

            DamageType damageType = ev.DamageHandler.Type;
            if (!Plugin.Instance.Config.WeaponHitToEvaporate.TryGetValue(damageType, out HitBoxEnums requiredHitbox))
                return;
            Log.Debug($"VVUP Weapon Evaporate: {ev.Player.Nickname} is dying from {damageType} damage. Required Hitbox: {requiredHitbox}");
            HitboxType hitboxType = HitboxType.Body;

            if (ev.DamageHandler.Base is PlayerStatsSystem.FirearmDamageHandler firearmDamageHandler)
            {
                hitboxType = firearmDamageHandler.Hitbox;
                Log.Debug($"VVUP Weapon Evaporate: FirearmDamageHandler detected, HitboxType: {hitboxType}");
            }
            
            bool shouldEvaporate = requiredHitbox switch
            {
                HitBoxEnums.Body => hitboxType == HitboxType.Body,
                HitBoxEnums.Headshot => hitboxType == HitboxType.Headshot,
                HitBoxEnums.Limb => hitboxType == HitboxType.Limb,
                HitBoxEnums.Any => true,
                _ => false
            };

            if (shouldEvaporate)
            {
                Log.Debug(
                    $"VVUP Weapon Evaporate: {ev.Player.Nickname} died with [{damageType}] with [{hitboxType}]. Required [{requiredHitbox}] hitbox, evaporating");
                ev.Player.Vaporize();
            }
        }
    }
}