using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Respawning;
using Respawning.Waves;

namespace VVUP.ScpChanges
{
    public class ScpChangesEventHandlers
    {
        public Plugin Plugin;
        public ScpChangesEventHandlers(Plugin plugin) => Plugin = plugin;

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player == null)
                return;
            if (ev.NewRole == RoleTypeId.Scp106 && Plugin.Instance.Config.OldScp106Behavior)
            {
                Log.Debug("VVUP SCP Changes: Old SCP 106 Behavior is enabled, setting health and damage resistance");
                ev.Player.MaxHealth = Plugin.Instance.Config.Scp106Health;
                ev.Player.Health = Plugin.Instance.Config.Scp106Health;
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (Plugin.Instance.ScpChangesEventHandlers == null)
                return;
            if (ev.Player == null || ev.Attacker == null)
                return;
            if (ev.Player.Role == RoleTypeId.Scp106 && Plugin.Instance.Config.OldScp106Behavior && ev.DamageHandler.Type == DamageType.Firearm)
            {
                if (!Plugin.Instance.Config.ResistanceWithHume && ev.Player.HumeShield > 0)
                {
                    Log.Debug("VVUP SCP Changes: Old SCP 106 Behavior is enabled, but Hume Shield is active, not applying damage resistance");
                    //Return won't work here since the OneShot possibility is outside of this if statement
                    //Technically I could invert the if statement with ResistanceWithHume being true, but I want the debug statements just incase things go wrong
                }
                else
                {
                    Log.Debug("VVUP SCP Changes: Old SCP 106 Behavior is enabled, setting damage resistance");
                    ev.Amount *= Plugin.Instance.Config.Scp106DamageResistance;
                    Log.Debug($"VVUP SCP Changes: Reduced damage to {ev.Amount}");
                }
            }
            if (ev.Attacker.Role == RoleTypeId.Scp106 && Plugin.Instance.Config.Scp106OneShot)
            {
                Log.Debug($"VVUP SCP Changes: SCP 106 One Shot is enabled, teleporting {ev.Player.Nickname} to pocket dimension");
                ev.Player.EnableEffect(EffectType.PocketCorroding);
            }
            if (ev.Attacker.Role == RoleTypeId.Scp049 && Plugin.Instance.Config.Scp049OneShot)
            {
                Log.Debug($"VVUP SCP Changes: SCP 049 One Shot is enabled, killing {ev.Player.Nickname}");
                ev.Player.Kill(DamageType.CardiacArrest);
            }
        }
        public void OnUsingItem(UsedItemEventArgs ev)
        {
            if (Plugin.Instance.ScpChangesEventHandlers == null)
                return;
            if (ev.Item.Type != ItemType.SCP1576)
                return;
            Log.Debug("VVUP SCP Changes: Item is SCP 1576");
            string Scp1576DisplayText = ProcessStringVariables(Plugin.Instance.Config.Scp1576Text);
            Log.Debug($"VVUP SCP Changes: Showing text to {ev.Player.Nickname}");
            ev.Player.ShowHint(Scp1576DisplayText, Plugin.Instance.Config.Scp1576TextDuration);
        }

        public string ProcessStringVariables(string raw)
        {
            Log.Debug("VVUP SCP Changes: Processing String Variables");
            
            float timeBeforeSpawn = float.MaxValue;
            bool foundActiveWave = false;
    
            foreach (TimeBasedWave wave in WaveManager.Waves)
            {
                if (wave.Timer.TimeLeft > 0 && wave.Timer.TimeLeft < timeBeforeSpawn)
                {
                    timeBeforeSpawn = wave.Timer.TimeLeft;
                    foundActiveWave = true;
                }
            }
            if (!foundActiveWave)
                timeBeforeSpawn = 0;
            
            string replacedText = raw 
                .Replace("%spectators%", Player.List.Count(p => p.Role.Type == RoleTypeId.Spectator).ToString())
                .Replace("%timebeforespawnwave%", Math.Floor(timeBeforeSpawn).ToString())
                .Replace("%customroles%", GetCustomRolesText())
                .Replace("%roles%", GetRolesText())
                .Replace("%teams%", GetTeamsText());
            
            return replacedText;
        }
        
        private string GetCustomRolesText()
        {
            string customRolesText = string.Empty;
            foreach (var role in Plugin.Instance.Config.Scp1576CustomRolesAlive)
            {
                CustomRole customRole = CustomRole.Get(role.Key);
                if (customRole != null && Player.List.Any(p => customRole.TrackedPlayers.Contains(p)))
                    customRolesText += role.Value + "\n";
            }
            return customRolesText;
        }

        private string GetRolesText()
        {
            string rolesText = string.Empty;
            foreach (var role in Plugin.Instance.Config.AliveRoles)
            {
                if (Player.List.Any(p => p.Role.Type == role.Key))
                    rolesText += role.Value + "\n";
            }
            return rolesText;
        }

        private string GetTeamsText()
        {
            string teamsText = string.Empty;
            foreach (var team in Plugin.Instance.Config.AliveTeams)
            {
                if (Player.List.Any(p => p.Role.Team == team.Key))
                    teamsText += team.Value + "\n";
            }
            return teamsText;
        }
    }
}