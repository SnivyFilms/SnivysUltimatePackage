using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features.Pickups;

namespace VVUP.CustomItems.API
{
    public static class StickyGrenadeApi
    {
        public static Dictionary<Pickup, bool> CollidedGrenades { get; } = new Dictionary<Pickup, bool>();
        
        // Registry of sticky grenade types with their validation functions
        private static readonly Dictionary<string, System.Func<Pickup, bool>> StickyGrenadeTypes = new Dictionary<string, System.Func<Pickup, bool>>();

        /// <summary>
        /// Registers a new sticky grenade type with a validation function
        /// </summary>
        /// <param name="typeName">Name of the grenade type</param>
        /// <param name="validationFunc">Function to check if a pickup is this type of sticky grenade</param>
        public static void RegisterStickyGrenade(string typeName, System.Func<Pickup, bool> validationFunc)
        {
            StickyGrenadeTypes[typeName] = validationFunc;
        }

        /// <summary>
        /// Unregisters a sticky grenade type
        /// </summary>
        public static void UnregisterStickyGrenade(string typeName)
        {
            StickyGrenadeTypes.Remove(typeName);
        }

        /// <summary>
        /// Checks if a pickup is a sticky grenade according to any registered type
        /// </summary>
        public static bool IsStickyGrenade(Pickup pickup)
        {
            return pickup != null && StickyGrenadeTypes.Values.Any(validationFunc => validationFunc(pickup));
        }

        /// <summary>
        /// Clears all tracked grenades (called on round end)
        /// </summary>
        public static void ClearTrackedGrenades()
        {
            CollidedGrenades.Clear();
        }
    }
}
