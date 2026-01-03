using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using VVUP.Base.API;
using Light = Exiled.API.Features.Toys.Light;

namespace VVUP.Base.EventHandlers
{
    public class CustomItemEventHandlers
    {
        private readonly Plugin Plugin;
        public CustomItemEventHandlers(Plugin plugin) => Plugin = plugin;

        private static readonly Dictionary<Pickup, Light> ActiveGlowEffects = new Dictionary<Pickup, Light>();
        
        public void OnRoundStarted()
        {
            foreach (Pickup pickup in Pickup.List)
                HandleGlowEffect(pickup);
        }

        public void AddGlow(PickupAddedEventArgs ev) => HandleGlowEffect(ev.Pickup);

        public void RemoveGlow(PickupDestroyedEventArgs ev)
        {
            if (ev.Pickup == null || ev.Pickup?.Base?.gameObject == null)
                return;
                
            if (ActiveGlowEffects.ContainsKey(ev.Pickup))
                RemoveGlowEffect(ev.Pickup);
        }

        public void OnWaitingForPlayers() => ClearAllGlowEffects();

        private void HandleGlowEffect(Pickup pickup)
        {
            if (pickup == null)
                return;
            
            if (CustomItem.TryGet(pickup, out CustomItem ci))
            {
                if (ci is ICustomItemGlow { HasCustomItemGlow: true } glowableItem)
                {
                    Log.Debug($"VVUP Base: Applying glow effect to pickup {pickup} for custom item {ci.Name} (ID: {ci.Id}) (Native)");
                    ApplyGlowEffect(pickup, glowableItem.CustomItemGlowColor, glowableItem.GlowRange, glowableItem.GlowIntensity, glowableItem.ShadowType, glowableItem.GlowOffset);
                }
                
                else if (Plugin.Config.EnableCompatibilityGlow && 
                         Plugin.Config.CustomItemGlow.FirstOrDefault(g => g.CustomItemId == ci.Id) is { } configGlow)
                {
                    Log.Debug($"VVUP Base: Applying config glow effect to pickup {pickup} for custom item {ci.Name} (ID: {ci.Id}) (Compatibility Glow)");
                    ApplyGlowEffect(pickup, configGlow.GetColor(), configGlow.GlowRange, configGlow.Intensity, configGlow.ShadowType, configGlow.GetOffset());
                }
            }
        }

        private void ApplyGlowEffect(Pickup pickup, Color glowColor, float range = 0.25f, float intensity = 1f, ICustomItemGlow.GlowShadowType shadowType = ICustomItemGlow.GlowShadowType.None, Vector3? offset = null)
        {
            if (ActiveGlowEffects.ContainsKey(pickup))
            {
                RemoveGlowEffect(pickup);
            }
            var actualOffset = offset ?? Vector3.zero;
            var light = Light.Create(pickup.Position);
            light.Color = glowColor;
            light.Intensity = intensity;
            light.Range = range;
            light.ShadowType = (LightShadows)shadowType;

            // WIP
            PositionConstraint positionConstraint = light.GameObject.AddComponent<PositionConstraint>();

            ConstraintSource source = new ConstraintSource {sourceTransform = pickup.Transform, weight = 1f};

            positionConstraint.AddSource(source);

            positionConstraint.translationOffset = actualOffset;
            positionConstraint.constraintActive = true;
            positionConstraint.locked = true;

            ActiveGlowEffects[pickup] = light;
            Log.Debug($"VVUP Base: Applied glow effect to pickup {pickup} with color {glowColor} and range {range}");
        }

        private void RemoveGlowEffect(Pickup pickup)
        {
            var light = ActiveGlowEffects[pickup];
            if (light != null && light.Base != null)
            {
                NetworkServer.Destroy(light.Base.gameObject);
                Log.Debug($"VVUP Base: Removed glow effect from pickup {pickup}");
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
                    Log.Debug($"VVUP Base: Clearing glow effect {light}.");
                    NetworkServer.Destroy(light.Base.gameObject);
                }
                catch
                {
                    Log.Debug("VVUP Base: I think I was supposed to catch something here in regards to clearing glow effects...");
                     // You know it would be extremely hilarious if I didn't do anything.
                }
            }
            ActiveGlowEffects.Clear();
        }
    }
}