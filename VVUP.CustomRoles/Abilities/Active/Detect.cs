using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class Detect : ActiveAbility
    {
        public override string Name { get; set; } = "Detect";

        public override string Description { get; set; } = "Detects Hostiles Near By.";

        public override float Duration { get; set; } = 0f;

        public override float Cooldown { get; set; } = 120f;

        public string Message = string.Empty;

        public float DetectRange { get; set; } = 30f;

        public string FoundTargetHeader { get; set; } = "Hostiles detected near by";

        public string MissingRole { get; set; } = "Unknown Role";
        
        public string NoTargets { get; set; } = "There is no detected hostiles near you";
        public bool ShowMissingRoles { get; set; } = true;
        public bool UseHints { get; set; } = true;
        public float MessageDuration { get; set; } = 10f;

        public Dictionary<RoleTypeId, string> RoleNames { get; set; } = new Dictionary<RoleTypeId, string>()
        {
            {RoleTypeId.Scientist, "Scientist"},
            {RoleTypeId.NtfCaptain, "MTF Captain"},
            {RoleTypeId.NtfPrivate, "MTF Private"},
            {RoleTypeId.NtfSergeant, "MTF Sergeant"},
            {RoleTypeId.NtfSpecialist, "MTF Specialist"},
            {RoleTypeId.FacilityGuard, "Facility Guard"},
            {RoleTypeId.ChaosConscript, "Chaos Conscript"},
            {RoleTypeId.ChaosMarauder, "Chaos Marauder"},
            {RoleTypeId.ChaosRepressor, "Chaos Repressor"},
            {RoleTypeId.ChaosRifleman, "Chaos Rifleman"},
            {RoleTypeId.ClassD, "Class D"},
            {RoleTypeId.Scp049, "SCP-049"},
            {RoleTypeId.Scp0492, "SCP-049-2"},
            {RoleTypeId.Scp079, "SCP-079"},
            {RoleTypeId.Scp096, "SCP-096"},
            {RoleTypeId.Scp106, "SCP-106"},
            {RoleTypeId.Scp173, "SCP-173"},
            {RoleTypeId.Scp939, "SCP-939"},
            {RoleTypeId.Scp3114, "SCP-3114"}
        };
        
        public Dictionary<uint, string> CustomRoleNames { get; set; } = new Dictionary<uint, string>()
        {
            { 32, "Biochemist" },
            { 46, "Lockpicker Class D" }
        };

        protected override void AbilityUsed(Player player)
        {
            ActivateDetect(player);
            Timing.CallDelayed(0.5f, () =>
            {
                if (UseHints)
                {
                    Log.Debug($"VVUP Custom Abilities: Showing hint to {player.Nickname}");
                    player.ShowHint(Message, MessageDuration);
                }
                else
                {
                    Log.Debug($"VVUP Custom Abilities: Sending broadcast to {player.Nickname}");
                    player.Broadcast((ushort)MessageDuration, Message);
                }
            });
        }

        private void ActivateDetect(Player ply)
        {
            Log.Debug("VVUP Custom Abilities: Activating Detect");
            List<Player> detectedPlayers = new List<Player>();
            Message = string.Empty;
            foreach (Player p in Player.List)
            {
                if (ply.IsCHI)
                {
                    if (Vector3.Distance(ply.Position, p.Position) <= DetectRange &&
                        (p.Role == RoleTypeId.Scientist || p.IsNTF || p.Role == RoleTypeId.FacilityGuard ||
                         p.IsScp || p.Role == RoleTypeId.Tutorial))
                        detectedPlayers.Add(p);
                }
                else if (ply.IsNTF)
                {
                    if (Vector3.Distance(ply.Position, p.Position) <= DetectRange &&
                        (p.IsCHI || p.Role == RoleTypeId.ClassD || p.IsScp || p.Role == RoleTypeId.Tutorial))
                        detectedPlayers.Add(p);
                }
                else
                {
                    if (Vector3.Distance(ply.Position, p.Position) <= DetectRange &&
                        (p.IsCHI || p.IsNTF || p.Role == RoleTypeId.ClassD || p.Role == RoleTypeId.Scientist))
                        detectedPlayers.Add(p);
                }
            }

            if (detectedPlayers.Count > 0)
            {
                Message = FoundTargetHeader;
                Log.Debug($"VVUP Custom Abilities: Showing detected players to {ply.Nickname}");
                foreach (Player detectedPlayer in detectedPlayers)
                {
                    if (CustomRole.TryGet(detectedPlayer, out IReadOnlyCollection<CustomRole> customRole) && 
                        customRole.Any() && CustomRoleNames.TryGetValue(customRole.First().Id, out string customRoleName))
                    {
                        Message += $"{customRoleName}\n";
                    }
                    else if (RoleNames.TryGetValue(detectedPlayer.Role, out string roleName))
                    {
                        Message += $"{roleName}\n";
                    }
                    else if (ShowMissingRoles)
                    {
                        Message += $"{MissingRole}\n";
                    }
                }
            }
            else
            {
                Message = NoTargets;
            }
        }
    }
}