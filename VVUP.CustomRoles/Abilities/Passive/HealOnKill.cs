using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class HealOnKill : PassiveAbility
    {
        private readonly Dictionary<Player, CoroutineHandle> activeHoTs = new ();

        public override string Name { get; set; } = "Heal on Kill";

        public override string Description { get; set; } = "Heals the player when they kill someone.";

        [Description("How much health to give the player.")]
        public float HealAmount { get; set; } = 25f;

        [Description("Whether or not this heal can exceed their max health.")]
        public bool HealOverMax { get; set; } = false;

        [Description("Whether or not this heal is applied gradually over time (true) or instantly (false)")]
        public bool HealOverTime { get; set; } = true;

        [Description("How long the heal over time effect lasts, if used.")]
        public float HealOverTimeDuration { get; set; } = 10f;

        [Description("How often (in seconds) the heal over time effect ticks, if used.")]
        public float HealOverTimeTickFrequency { get; set; } = 1.0f;

        [Description("Whether or not the heal over time effect is ended early if the player takes damage.")]
        public bool DamageInterruptsHot { get; set; } = true;

        protected override void AbilityAdded(Player player)
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;
            if (HealOverTime && DamageInterruptsHot)
                Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.AbilityAdded(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            if (HealOverTime && DamageInterruptsHot) 
                Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.AbilityRemoved(player);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (Check(ev.Attacker))
            {
                if (HealOverTime)
                    activeHoTs[ev.Attacker] = Timing.RunCoroutine(DoHealOverTime(ev.Attacker));
                ev.Attacker.Heal(HealAmount, HealOverMax);
            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (Check(ev.Player))
            {
                if (DamageInterruptsHot && activeHoTs.ContainsKey(ev.Player))
                    Timing.KillCoroutines(activeHoTs[ev.Player]);
            }
        }

        private IEnumerator<float> DoHealOverTime(Player player)
        {
            float tickAmount = HealAmount / HealOverTimeDuration;
            int tickCount = Mathf.FloorToInt(HealOverTimeDuration / HealOverTimeTickFrequency);

            for (int i = 0; i < tickCount; i++)
            {
                player.Heal(tickAmount, HealOverMax);
                yield return Timing.WaitForSeconds(HealOverTimeTickFrequency);
            }
        }
    }
}