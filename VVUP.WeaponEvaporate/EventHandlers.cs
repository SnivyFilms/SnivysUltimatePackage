using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.WeaponEvaporate
{
    public class EventHandlers
    {
        public Plugin Plugin;
        public EventHandlers(Plugin plugin) => Plugin = plugin;
        
        private Dictionary<int, (int PlayerId, DamageType DamageType, HitboxType HitboxType)> _recentHits = 
            new Dictionary<int, (int, DamageType, HitboxType)>();
        public enum HitBoxEnums
        {
            Body, Headshot, Limb
        }

        public void OnShot(ShotEventArgs ev)
        {
            if (Plugin.Instance.EventHandlers == null)
                return;
            if (!Plugin.Instance.Config.IsEnabled)
                return;
            if (ev.Target == null || ev.Player == null)
                return;
            if (ev.Hitbox == null || ev.Player.CurrentItem == null)
                return;
            
            DamageType damageType = GetDamageTypeFromItem(ev.Player.CurrentItem.Type);
            
            _recentHits[ev.Target.Id] = (ev.Target.Id, damageType, ev.Hitbox.HitboxType);
        }

        public void OnHurt(HurtEventArgs ev)
        {
            if (Plugin.Instance.EventHandlers == null)
                return;
            if (!Plugin.Instance.Config.IsEnabled)
                return;
            if (ev.Player == null || ev.Attacker == null)
                return;
            if (ev.DamageHandler.Type == DamageType.Firearm)
                return;
            
            HitboxType hitboxType = HitboxType.Body;
            
            _recentHits[ev.Player.Id] = (ev.Player.Id, ev.DamageHandler.Type, hitboxType);
        }
        public void OnDying(DyingEventArgs ev)
        {
            if (Plugin.Instance.EventHandlers == null)
                return;
            if (!Plugin.Instance.Config.IsEnabled)
                return;
            Log.Debug("VVUP Weapon Evaporate: Weapon Evaporate is enabled, checking damage type");

            DamageType damageType = ev.DamageHandler.Type;
           
            if (Plugin.Instance.Config.WeaponHitToEvaporate.TryGetValue(damageType, out HitBoxEnums requiredHitbox))
            {
                if (_recentHits.TryGetValue(ev.Player.Id, out var hitInfo) && hitInfo.DamageType == damageType)
                {
                    bool shouldEvaporate = false;

                    switch (requiredHitbox)
                    {
                        case HitBoxEnums.Body:
                            shouldEvaporate = hitInfo.HitboxType == HitboxType.Body;
                            break;
                        case HitBoxEnums.Headshot:
                            shouldEvaporate = hitInfo.HitboxType == HitboxType.Headshot;
                            break;
                        case HitBoxEnums.Limb:
                            shouldEvaporate = hitInfo.HitboxType == HitboxType.Limb;
                            break;
                    }
                    
                    if (shouldEvaporate)
                    {
                        Log.Debug($"VVUP Weapon Evaporate: {ev.Player.Nickname} killed with {damageType} to {hitInfo.HitboxType}, evaporating");
                        ev.Player.Vaporize();
                    }
                    
                    _recentHits.Remove(ev.Player.Id);
                }
            }
        }
        
        private DamageType GetDamageTypeFromItem(ItemType itemType) =>
            DamageTypeExtensions.ItemConversion.TryGetValue(itemType, out var damageType) ? damageType : DamageType.Unknown;
    }
}