using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class RemoveDisguise : ActiveAbility
    {
        public override string Name { get; set; } = "Remove Disguise";

        public override string Description { get; set; } =
            "This removes your disguise, once it's off, you cannot put it back on, activate carefully";

        public override float Duration { get; set; } = 0f;
        public override float Cooldown { get; set; } = 5f;
        
        [Description("Sometimes the players inventory will be dropped, set this to true to automatically give it back")]
        public bool RestorePreviousInventory { get; set; } = false;

        [Description("This allows a player to undisguise into a custom role if true.")]
        public bool UseCustomRoles { get; set; } = false;

        [Description(
            "If UseCustomRoles is false, this will determine which normal role the player will undisguise into.")]
        public RoleTypeId UndisguiseRole { get; set; } = RoleTypeId.Tutorial;

        [Description(
            "If UseCustomRoles is true, this will determine which custom role the player will undisguise into by Custom Role ID.")]
        public uint UndisguiseCustomRole { get; set; } = 0;

        [Description("If true, the player's position will be saved and restored after the ability is used, useful for Custom Roles since those will always set to the custom role spawn point.")]
        public bool SavePosition { get; set; } = true;
        

        protected override void AbilityUsed(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Removing {player.Nickname} disguise");
            List<Item> storedInventory = player.Items.ToList();

            var ammoCount = player.Ammo.ToDictionary(ammo => ammo.Key, ammo => ammo.Value);

            Vector3 savedPosition = player.Position;
            if (UseCustomRoles)
            {
                CustomRole.Get(UndisguiseCustomRole)?.AddRole(player);
                if (SavePosition)
                    Timing.CallDelayed(0.25f, () => player.Position = savedPosition);

                Log.Debug($"VVUP Custom Abilities: {player.Nickname} undisguised into custom role into {CustomRole.Get(UndisguiseCustomRole)?.Name})");
            }
            else
            {
                RoleSpawnFlags spawnFlags;

                if (SavePosition && RestorePreviousInventory)
                    spawnFlags = RoleSpawnFlags.All;
                else if (SavePosition)
                    spawnFlags = RoleSpawnFlags.AssignInventory;
                else if (RestorePreviousInventory)
                    spawnFlags = RoleSpawnFlags.UseSpawnpoint;
                else
                    spawnFlags = RoleSpawnFlags.None;
                player.Role.Set(UndisguiseRole, spawnFlags);
                Log.Debug($"VVUP Custom Abilities: {player.Nickname} undisguised into role {UndisguiseRole}");
            }
            
            if (RestorePreviousInventory)
            {
                Timing.CallDelayed(1f, () =>
                {
                    player.ClearInventory();
                    
                    foreach (Item item in storedInventory)
                    {
                        player.AddItem(item);
                    }

                    foreach (KeyValuePair<ItemType, ushort> ammo in ammoCount)
                    {
                        player.AddAmmo((AmmoType)ammo.Key, ammo.Value);
                    }
                });
            }
        }
    }
}