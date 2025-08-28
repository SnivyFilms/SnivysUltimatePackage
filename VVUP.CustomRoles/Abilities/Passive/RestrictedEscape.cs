using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class RestrictedEscape : PassiveAbility
    {
        public override string Name { get; set; } = "Restricted Escape";

        public override string Description { get; set; } =
            "Prevents players from escaping regularly (can still escape while detained)";
        public List<Player> PlayersWithRestrictedEscapeEffect = new List<Player>();

        [Description("The text that is shown if the player is uncuffed, and needs to be cuffed to escape")]
        public string EscapeTextUncuffed { get; set; } = "You're unable to escape unless you're detained";
        [Description("The text that is shown if the player is cuffed, and needs to be uncuffed to escape")]
        public string EscapeTextCuffed { get; set; } = "You're unable to escape unless you're not detained";
        [Description("The text that is shown if the player is unable to escape as a whole.")]
        public string EscapeTextBoth { get; set; } = "You're unable to escape";
        public float EscapeTextTime { get; set; } = 5;
        public bool UseHints { get; set; } = true;
        public bool AllowedUncuffedEscape { get; set; } = false;
        public bool AllowedCuffedEscape { get; set; } = false;
        
        protected override void AbilityAdded(Player player)
        {
            PlayersWithRestrictedEscapeEffect.Add(player);
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
        }
        protected override void AbilityRemoved(Player player)
        {
            PlayersWithRestrictedEscapeEffect.Remove(player);
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;
        }

        private void OnEscaping(EscapingEventArgs ev)
        {
            if (PlayersWithRestrictedEscapeEffect.Contains(ev.Player) && !AllowedCuffedEscape && !AllowedUncuffedEscape)
            {
                ev.IsAllowed = false;
                Log.Debug($"VVUP Custom Abilities: Restricting Escape of {ev.Player.Nickname}");
                if (UseHints)
                    ev.Player.ShowHint(EscapeTextBoth, EscapeTextTime);
                else
                    ev.Player.Broadcast((ushort)EscapeTextTime, EscapeTextBoth);
            }
            else if (PlayersWithRestrictedEscapeEffect.Contains(ev.Player) && !AllowedCuffedEscape && ev.Player.IsCuffed)
            {
                ev.IsAllowed = false;
                Log.Debug($"VVUP Custom Abilities: Restricting Escape of {ev.Player.Nickname} while cuffed");
                if (UseHints)
                    ev.Player.ShowHint(EscapeTextCuffed, EscapeTextTime);
                else
                    ev.Player.Broadcast((ushort)EscapeTextTime, EscapeTextCuffed);
            }
            else if (PlayersWithRestrictedEscapeEffect.Contains(ev.Player) && !AllowedUncuffedEscape && !ev.Player.IsCuffed)
            {
                ev.IsAllowed = false;
                Log.Debug($"VVUP Custom Abilities: Restricting Escape of {ev.Player.Nickname} while uncuffed");
                if (UseHints)
                    ev.Player.ShowHint(EscapeTextUncuffed, EscapeTextTime);
                else
                    ev.Player.Broadcast((ushort)EscapeTextTime, EscapeTextUncuffed);
            }
        }
    }
}