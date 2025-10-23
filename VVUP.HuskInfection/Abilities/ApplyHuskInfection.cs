using System.ComponentModel;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using VVUP.HuskInfection.EventHandlers;

namespace VVUP.HuskInfection.Abilities
{
    public class ApplyHuskInfection : PassiveAbility
    {
        public override string Name { get; set; } = "Husk Infection";
        public override string Description { get; set; } = "When you hit a player, they have a chance to be infected with Husk Infection, which will slowly take over their body and turn them into a Husk.";
        [Description("A chance to infect a player, 0 to 100%")]
        public float InfectionChance { get; set; } = 50f;
        [Description("How long it takes for the infection to reach stage 1, in seconds")]
        public float InfectionStageOneDelay { get; set; } = 30f;
        [Description("How long it takes for the infection to reach stage 2, in seconds **AFTER** stage 1 is reached")]
        public float InfectionStageTwoDelay { get; set; } = 90f;
        [Description("The text that will be shown once stage 2 is reached")]
        public string InfectionText { get; set; } = "<color=red><size=30>You feel something in your throat. You try to scream, but nothing comes out.</size></color>";
        public bool UseHints { get; set; } = false;
        public float TextDisplayTime { get; set; } = 10f;
        public uint HuskZombieCustomRoleId { get; set; } = 56;
        public string HuskTakeOverDeathReason { get; set; } = "You have been taken over by a Husk Infection.";
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: ApplyHuskInfection, Adding ApplyHuskInfection Ability to {player.Nickname}");
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: ApplyHuskInfection, Removing ApplyHuskInfection Ability from {player.Nickname}");
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null)
                return;
            if (Check(ev.Attacker) == Check(ev.Player))
                return;
            if (Check(ev.Attacker) && Base.GetRandomNumber.GetRandomInt(0, 101) < InfectionChance)
            {
                Log.Debug($"VVUP Custom Abilities: ApplyHuskInfection, {ev.Attacker.Nickname} hit {ev.Player.Nickname}, applying Husk Infection.");
                HuskInfectionEventHandlers huskInfection = new HuskInfectionEventHandlers(ev.Player, InfectionStageOneDelay, InfectionStageTwoDelay, 
                    InfectionText, UseHints, TextDisplayTime, HuskZombieCustomRoleId, HuskTakeOverDeathReason);
            }
        }
    }
}