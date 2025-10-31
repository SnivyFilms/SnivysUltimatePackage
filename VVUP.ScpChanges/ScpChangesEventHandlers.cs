using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using Respawning;
using Respawning.Waves;

namespace VVUP.ScpChanges
{
    public class ScpChangesEventHandlers
    {
        public Plugin Plugin;
        public ScpChangesEventHandlers(Plugin plugin) => Plugin = plugin;

        public void OnChangingRole(SpawnedEventArgs ev)
        {
            if (ev.Player == null)
                return;
            if (ev.Player.Role == RoleTypeId.Scp106 && Plugin.Instance.Config.OldScp106Behavior)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    Log.Debug("VVUP SCP Changes: Old SCP 106 Behavior is enabled, setting health and damage resistance");
                    ev.Player.MaxHealth = Plugin.Instance.Config.Scp106Health;
                    ev.Player.Health = Plugin.Instance.Config.Scp106Health;
                });
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (Plugin.Instance.ScpChangesEventHandlers == null)
                return;
            if (ev.Player == null || ev.Attacker == null)
                return;
            if (ev.Player == ev.Attacker)
                return;
            if (ev.Player.Role == RoleTypeId.Scp106 && Plugin.Instance.Config.OldScp106Behavior && ev.DamageHandler.Type is
                    DamageType.A7 or DamageType.AK or DamageType.Com15 or DamageType.Com18 or DamageType.Com45 or DamageType.Crossvec or
                    DamageType.E11Sr or DamageType.Frmg0 or DamageType.Fsp9 or DamageType.Revolver or DamageType.Shotgun or DamageType.Logicer or DamageType.Scp127)
            {
                if (!Plugin.Instance.Config.ResistanceWithHume && ev.Player.HumeShield > 0)
                {
                    Log.Debug("VVUP SCP Changes: Old SCP 106 Behavior is enabled, but Hume Shield is active, not applying damage resistance");
                }
                else if (Plugin.Instance.Config.ResistanceWithHume || ev.Player.HumeShield <= 0)
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
                ev.Amount = ev.Player.Health + ev.Player.ArtificialHealth + ev.Player.HumeShield + 1;
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
    
            foreach (var wave in WaveManager.Waves.Cast<TimeBasedWave>().Where(wave => wave.Timer.TimeLeft > 0 && wave.Timer.TimeLeft < timeBeforeSpawn))
            {
                timeBeforeSpawn = wave.Timer.TimeLeft;
                foundActiveWave = true;
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
            return (from role in Plugin.Instance.Config.Scp1576CustomRolesAlive let customRole = CustomRole.Get(role.Key) where customRole != null && Player.List.Any(p => customRole.TrackedPlayers.Contains(p)) select role).Aggregate(string.Empty, (current, role) => current + (role.Value + "\n"));
        }

        private string GetRolesText()
        {
            return Plugin.Instance.Config.AliveRoles.Where(role => Player.List.Any(p => p.Role.Type == role.Key)).Aggregate(string.Empty, (current, role) => current + (role.Value + "\n"));
        }

        private string GetTeamsText()
        {
            return Plugin.Instance.Config.AliveTeams.Where(team => Player.List.Any(p => p.Role.Team == team.Key)).Aggregate(string.Empty, (current, team) => current + (team.Value + "\n"));
        }
    }
}