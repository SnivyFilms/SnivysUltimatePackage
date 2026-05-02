using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Warhead;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp096;
using Respawning;
using Respawning.Waves;
using Scp096Role = Exiled.API.Features.Roles.Scp096Role;

namespace VVUP.ScpChanges
{
    public class ScpChangesEventHandlers
    {
        /*
        public void OnNukeStarted(StartingEventArgs ev)
        {
            if (!Plugin.Config.Scp096UnlimitedRageDuringNuke)
                return;
            _scp096RageCoroutine = Timing.RunCoroutine(InfiniteRage());
        }
        private IEnumerator<float> InfiniteRage()
        {
            Log.Debug("VVUP SCP Changes: Starting InfiniteRage coroutine for SCP 096");
            yield return Timing.WaitForSeconds(1f);
            for(;;)
            {
                foreach (var player in Player.List.Where(p => p.Role.Type == RoleTypeId.Scp096))
                {
                    var scp096Role = player.Role as Scp096Role;
                    if (scp096Role == null)
                        continue;
                    if (!Warhead.IsInProgress || Warhead.IsDetonated)
                    {
                        if (scp096Role.Targets.Count == 0)
                        {
                            Log.Debug($"VVUP SCP Changes: Nuke ended and SCP 096 {player.Nickname} has no targets, calming down.");
                            scp096Role.Calm();
                        }
                        yield break;
                    }
                    if (scp096Role.RageState != Scp096RageState.Enraged)
                    {
                        Log.Debug($"VVUP SCP Changes: Nuke in progress, setting SCP 096 {player.Nickname} rage to nuke timer");
                        scp096Role.RageManager.ServerEnrage(Warhead.DetonationTimer);
                        scp096Role.EnragedTimeLeft = Warhead.DetonationTimer;
                        scp096Role.TotalEnrageTime = Warhead.DetonationTimer;
                    }
                }
                Log.Debug($"VVUP SCP Changes: Waiting 1 second before next SCP-096 rage update.");
                yield return Timing.WaitForSeconds(1f);
            }
        }*/
        
        public Plugin Plugin;
        public ScpChangesEventHandlers(Plugin plugin) => Plugin = plugin;
        private CoroutineHandle _scp096RageCoroutine;
        
        public void OnChangingRole(SpawnedEventArgs ev)
        {
            if (ev.Player == null)
                return;
           
            Timing.CallDelayed(0.1f, () =>
            {
                if (Plugin.Instance.Config.ScpHealths.Any(s => s.ApplyToCustomRoles && s.Role == ev.Player.Role) 
                    || Plugin.Instance.Config.ScpHealths.Any(s => !s.ApplyToCustomRoles && s.Role == ev.Player.Role && CustomRole.TryGet(ev.Player, out var roles) && roles.IsEmpty()))
                {
                    var scpHealth = Plugin.Instance.Config.ScpHealths.First(s => s.Role == ev.Player.Role);
                    Log.Debug($"VVUP SCP Changes: Setting health for {ev.Player.Nickname} to {scpHealth.MaxHealth}, setting hume to {scpHealth.HumeShield} and setting hume regen multiplier to {scpHealth.HumeShieldRegenMultiplier}");
                    ev.Player.MaxHealth = scpHealth.MaxHealth;
                    ev.Player.Health = scpHealth.MaxHealth;
                    ev.Player.MaxHumeShield = scpHealth.HumeShield;
                    ev.Player.HumeShieldRegenerationMultiplier = scpHealth.HumeShieldRegenMultiplier;
                }
            });
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player == null || ev.Attacker == null || ev.Player == ev.Attacker)
                return;
            
            var resistance = Plugin.Instance.Config.ScpDamageResistances
                .FirstOrDefault(s =>
                    s.Role == ev.Player.Role.Type &&
                    s.DamageType == ev.DamageHandler.Type);

            if (resistance != null)
            {
                if (CustomRole.TryGet(ev.Player, out _) && !resistance.ShouldApplyToCustomRoles)
                {
                    Log.Debug(
                        $"VVUP SC: {ev.Player.Nickname} is a role that would get a damage resistance, but ShouldApplyToCustomRoles is false and they have a custom role, not applying resistance.");
                    return;
                }
                
                if (!resistance.ShouldResistWithHume && ev.Player.HumeShield > 0)
                {
                    Log.Debug($"VVUP SC: {ev.Player.Nickname} is a role that would get a damage resistance, but they have hume shield, not applying damage resistance");
                    return;
                }
                Log.Debug($"VVUP SC: {ev.Player.Nickname} is taking damage of type {ev.DamageHandler.Type} and has a resistance modifier of {resistance.ResistanceModifier}, reducing damage from {ev.Amount} to {ev.Amount * resistance.ResistanceModifier}");
                ev.Amount *= resistance.ResistanceModifier;
                return;
            }

            var damageBonus = Plugin.Instance.Config.ScpSetDamages
                .FirstOrDefault(s => s.Role == ev.Attacker.Role);
            if (damageBonus != null)
            {
                float damageToApply = damageBonus.Damage;
                if (!damageBonus.ShouldApplyToCustomRoles && CustomRole.TryGet(ev.Attacker, out _))
                {
                    Log.Debug(
                        $"VVUP SC: {ev.Attacker.Nickname} is a role that would get a damage bonus, but they have a custom role and ShouldApplyToCustomRoles is false, not applying damage bonus.");
                    return;
                }
                
                if (damageBonus.ShouldOneShotApplyEffects && ev.Attacker.Role == RoleTypeId.Scp106)
                {
                    Log.Debug($"VVUP SC: {ev.Attacker.Nickname} is attacking as SCP 106 and ShouldOneShotEffects is true, teleporting {ev.Player.Nickname} to pocket dimension.");
                    ev.Player.EnableEffect(EffectType.PocketCorroding);
                }

                if (damageBonus.DamageMultipliersAgainstSpecificRoles != null &&
                    damageBonus.DamageMultipliersAgainstSpecificRoles.ContainsKey(ev.Player.Role))
                {
                    Log.Debug($"VVUP SC: {ev.Attacker.Nickname} is attacking {ev.Player.Nickname} and has a specific damage multiplier against their role, multiplying damage by {damageBonus.DamageMultipliersAgainstSpecificRoles[ev.Player.Role]}");
                    damageToApply *= damageBonus.DamageMultipliersAgainstSpecificRoles[ev.Player.Role];
                }

                Log.Debug($"VVUP SC: Applying {damageToApply} to {ev.Player.Nickname}.");
                ev.Amount = damageToApply;
            }
        }
        public void OnUsingItem(UsedItemEventArgs ev)
        {
            if (ev.Item.Type != ItemType.SCP1576)
                return;
            Log.Debug("VVUP SCP Changes: Item is SCP 1576");
            if (String.IsNullOrWhiteSpace(Plugin.Instance.Config.Scp1576Text))
            {
                Log.Debug("VVUP SCP Changes: No SCP 1576 text configured, not showing hint.");
                return;
            }

            string scp1576DisplayText = ProcessStringVariables(Plugin.Instance.Config.Scp1576Text);
            Log.Debug($"VVUP SCP Changes: Showing text to {ev.Player.Nickname}");
            ev.Player.ShowHint(scp1576DisplayText, Plugin.Instance.Config.Scp1576TextDuration);
        }
        
        public string ProcessStringVariables(string raw)
        {
            Log.Debug("VVUP SCP Changes: Processing String Variables");

            float timeBeforeSpawn = WaveManager.Waves
                .OfType<TimeBasedWave>()
                .Select(wave => wave.Timer.TimeLeft)
                .Where(timeLeft => timeLeft > 0)
                .DefaultIfEmpty(0f)
                .Min();

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
            return (from role in Plugin.Instance.Config.Scp1576CustomRolesAlive let customRole = CustomRole.Get(role.Key) 
                where customRole != null && Player.List.Any(p => customRole.TrackedPlayers.Contains(p)) select role)
                .Aggregate(string.Empty, (current, role) => current + (role.Value + "\n"));
        }

        private string GetRolesText()
        {
            return Plugin.Instance.Config.AliveRoles.Where(role => Player.List.Any(p => p.Role.Type == role.Key))
                .Aggregate(string.Empty, (current, role) => current + (role.Value + "\n"));
        }

        private string GetTeamsText()
        {
            return Plugin.Instance.Config.AliveTeams.Where(team => Player.List.Any(p => p.Role.Team == team.Key))
                .Aggregate(string.Empty, (current, team) => current + (team.Value + "\n"));
        }
    }
}