using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class CustomRoleEscape : PassiveAbility
    {
        public override string Name { get; set; } = "Custom Role Escape";

        public override string Description { get; set; } =
            "Handles if you are a custom role, if you escape are you given another custom role";
        public bool EscapeToRegularRole { get; set; } = false;
        public RoleTypeId RegularRole { get; set; } = RoleTypeId.Tutorial;
        public String UncuffedEscapeCustomRole { get; set; } = String.Empty;
        public String CuffedEscapeCustomRole { get; set; } = String.Empty;
        public bool AllowUncuffedCustomRoleChange { get; set; } = true;
        public bool AllowCuffedCustomRoleChange { get; set; } = true;
        public bool SaveInventory { get; set; } = true;
        [Description("Should OnSpawn be used if the player is uncuffed. Useful if there is custom escapes.")]
        public bool UseOnSpawnUncuffedEscape { get; set; } = false;
        [Description("Should OnSpawn be used if the player is cuffed. Useful if there is custom escapes.")]
        public bool UseOnSpawnCuffedEscape { get; set; } = false;

        private List<Item> storedInventory = new List<Item>();
        
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Custom Role Escape, Adding Custom Role Escape Ability to {player.Nickname}");
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Player.ChangingRole += OnRoleChange;
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Custom Role Escape, Removing Custom Role Escape Ability from {player.Nickname}");
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;
            Exiled.Events.Handlers.Player.ChangingRole -= OnRoleChange;
        }

        private void OnEscaping(EscapingEventArgs ev)
        {
            Log.Debug($"VVUP Custom Abilities: Processing {ev.Player.Nickname} custom escape");
            storedInventory = ev.Player.Items.ToList();
            
            if (ev.Player.IsCuffed && AllowCuffedCustomRoleChange && CuffedEscapeCustomRole != String.Empty && !UseOnSpawnCuffedEscape)
            {
                ev.IsAllowed = false;
                CustomRole.Get(CuffedEscapeCustomRole)?.AddRole(ev.Player);
                storedInventory.Clear();
            }
            else if (AllowUncuffedCustomRoleChange && UncuffedEscapeCustomRole != String.Empty && !UseOnSpawnUncuffedEscape)
            {
                ev.IsAllowed = false;
                CustomRole.Get(UncuffedEscapeCustomRole)?.AddRole(ev.Player);
                if (SaveInventory)
                {
                    Timing.CallDelayed(1f, () =>
                    {
                        foreach (Item item in storedInventory)
                        {
                            item.CreatePickup(ev.Player.Position);
                        }
                        storedInventory.Clear();
                    });
                }
                else
                    storedInventory.Clear();
            }
            else if (EscapeToRegularRole && !UseOnSpawnUncuffedEscape)
            {
                ev.IsAllowed = false;
                ev.Player.Role.Set(RegularRole);
                if (SaveInventory)
                {
                    Timing.CallDelayed(1f, () =>
                    {
                        foreach (Item item in storedInventory)
                        {
                            item.CreatePickup(ev.Player.Position);
                        }
                        storedInventory.Clear();
                    });
                }
                else
                    storedInventory.Clear();
            }
            else
            {
                Log.Debug($"VVUP Custom Abilities: {ev.Player.Nickname} did not escape with a custom role, continuing normal escape.");
            }
        }

        private void OnRoleChange(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole is RoleTypeId.Spectator or RoleTypeId.None or RoleTypeId.Overwatch)
                return;
            Log.Debug($"VVUP Custom Abilities: Processing {ev.Player.Nickname} custom escape");
            if (UseOnSpawnUncuffedEscape && ev.Reason == SpawnReason.Escaped && UncuffedEscapeCustomRole != String.Empty)
            {
                ev.SpawnFlags = RoleSpawnFlags.UseSpawnpoint;
                ev.Player.ClearInventory();
                CustomRole.Get(UncuffedEscapeCustomRole)?.AddRole(ev.Player);
                if (SaveInventory)
                {
                    Timing.CallDelayed(1f, () =>
                    {
                        foreach (Item item in storedInventory)
                        {
                            item.CreatePickup(ev.Player.Position);
                        }
                        storedInventory.Clear();
                    });
                }
                else
                    storedInventory.Clear();
            }
            else if (UseOnSpawnCuffedEscape && ev.Reason == SpawnReason.Escaped && CuffedEscapeCustomRole != String.Empty)
            {
                ev.SpawnFlags = RoleSpawnFlags.UseSpawnpoint;
                CustomRole.Get(CuffedEscapeCustomRole)?.AddRole(ev.Player);
                storedInventory.Clear();
            }
            else if (EscapeToRegularRole && UseOnSpawnUncuffedEscape)
            {
                ev.IsAllowed = false;
                ev.Player.Role.Set(RegularRole);
                if (SaveInventory)
                {
                    Timing.CallDelayed(1f, () =>
                    {
                        foreach (Item item in storedInventory)
                        {
                            item.CreatePickup(ev.Player.Position);
                        }
                        storedInventory.Clear();
                    });
                }
                else
                    storedInventory.Clear();
            }
        }
    }
}