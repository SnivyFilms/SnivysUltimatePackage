﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using MEC;
using PlayerRoles;

namespace SnivysCustomRolesAbilities.Abilities
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

        protected override void AbilityUsed(Player player)
        {
            List<Item> storedInventory = player.Items.ToList();

            var ammoCount = player.Ammo.ToDictionary(ammo => ammo.Key, ammo => ammo.Value);


            if (player.Role == RoleTypeId.ClassD || player.Role == RoleTypeId.ChaosConscript ||
                player.Role == RoleTypeId.ChaosMarauder || player.Role == RoleTypeId.ChaosRepressor ||
                player.Role == RoleTypeId.ChaosRifleman)
                player.Role.Set(RoleTypeId.NtfSergeant);
            else if (player.Role == RoleTypeId.Scientist || player.Role == RoleTypeId.FacilityGuard ||
                     player.Role == RoleTypeId.NtfCaptain || player.Role == RoleTypeId.NtfPrivate ||
                     player.Role == RoleTypeId.NtfSergeant || player.Role == RoleTypeId.NtfSpecialist)
                player.Role.Set(RoleTypeId.ChaosRifleman);

            if (RestorePreviousInventory)
            {
                Timing.CallDelayed(1f, () =>
                {
                    foreach (Item item in storedInventory)
                    {
                        player.AddItem(item);
                    }

                    foreach (KeyValuePair<ItemType, ushort> ammo in ammoCount)
                    {
                        player.AddAmmo(ammoCount);
                    }
                });
            }
        }
    }
}