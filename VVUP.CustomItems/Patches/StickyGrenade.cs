using HarmonyLib;
using InventorySystem.Items.Pickups;
using Exiled.API.Features.Pickups;
using UnityEngine;
using VVUP.CustomItems.API;

namespace VVUP.CustomItems.Patches
{
    [HarmonyPatch(typeof(CollisionDetectionPickup), nameof(CollisionDetectionPickup.OnCollisionEnter))]
    internal static class StickyGrenadeCollision
    {
        static bool Prefix(CollisionDetectionPickup __instance, Collision collision)
        {
            Pickup pickup = Pickup.Get(__instance.gameObject);
            if (pickup == null)
                return true;

            if (StickyGrenadeApi.IsStickyGrenade(pickup) && !StickyGrenadeApi.CollidedGrenades.ContainsKey(pickup))
            {
                StickyGrenadeApi.CollidedGrenades[pickup] = true;
            }
            
            return true;
        }
    }

    [HarmonyPatch(typeof(PickupStandardPhysics), nameof(PickupStandardPhysics.ServerSendFreeze), MethodType.Getter)]
    internal static class StickyGrenadeFreezing
    {
        static void Postfix(PickupStandardPhysics __instance, ref bool __result)
        {
            if (__instance?.Rb == null)
                return;

            Pickup pickup = Pickup.Get(__instance.Rb.gameObject);
            if (pickup == null)
                return;

            bool isStickyGrenade = StickyGrenadeApi.IsStickyGrenade(pickup);
            bool hasCollided = StickyGrenadeApi.CollidedGrenades.ContainsKey(pickup);

            if (isStickyGrenade && hasCollided)
            {
                __result = true;
                __instance.Rb.isKinematic = true;
            }
        }
    }
}