using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Mirror;
using UnityEngine;
using VVUP.CustomItems.API;
using Light = Exiled.API.Features.Toys.Light;

namespace VVUP.CustomItems.EventHandlers
{
    public class CustomItemEventHandlers
    {
        private readonly Plugin Plugin;
        public CustomItemEventHandlers(Plugin plugin) => Plugin = plugin;
        
        private static readonly Dictionary<Pickup, Light> ActiveGlowEffects = new Dictionary<Pickup, Light>();
        
        public void OnRoundStarted()
        {
            foreach (Pickup pickup in Pickup.List)
            {
                CustomItem.TryGet(pickup, out CustomItem ci);
                if (ci is ICustomItemGlow { HasCustomItemGlow: true } glowableItem)
                {
                    ApplyGlowEffect(pickup, glowableItem.CustomItemGlowColor);
                }
            }
        }
        
        public void AddGlow(PickupAddedEventArgs ev)
        {
            CustomItem.TryGet(ev.Pickup, out CustomItem ci);
            if (ci is ICustomItemGlow { HasCustomItemGlow: true } glowableItem)
            {
                ApplyGlowEffect(ev.Pickup, glowableItem.CustomItemGlowColor);
            }
        }
        
        public void RemoveGlow(PickupDestroyedEventArgs ev)
        {
            if (ev.Pickup == null || ev.Pickup?.Base?.gameObject == null)
                return;
            if (!ActiveGlowEffects.ContainsKey(ev.Pickup)) 
                return;
            if (CustomItem.TryGet(ev.Pickup.Serial, out CustomItem ci) && ci is ICustomItemGlow { HasCustomItemGlow: true })
            {
                RemoveGlowEffect(ev.Pickup);
            }
        }
        
        public void OnWaitingForPlayers()
        {
            ClearAllGlowEffects();
        }

        private void ApplyGlowEffect(Pickup pickup, Color32 glowColor)
        {
            var light = Light.Create(pickup.Position);
            light.Color = glowColor;
            light.Range = 0.25f;
            light.ShadowType = LightShadows.None;
            light.Base.gameObject.transform.SetParent(pickup.Base.gameObject.transform);
            ActiveGlowEffects[pickup] = light;
        }

        private void RemoveGlowEffect(Pickup pickup)
        {
            var light = ActiveGlowEffects[pickup];
            if (light != null && light.Base != null)
            {
                NetworkServer.Destroy(light.Base.gameObject);
            }
            ActiveGlowEffects.Remove(pickup);
        }

        private void ClearAllGlowEffects()
        {
            foreach (var light in ActiveGlowEffects.Select(lights => lights.Value)
                         .Where(light => light != null && light.Base != null))
            {
                try
                {
                    NetworkServer.Destroy(light.Base.gameObject);
                }
                catch
                {
                     // You know it would be really funny if I didn't do anything.
                }
            }
            ActiveGlowEffects.Clear();
        }
    }
}