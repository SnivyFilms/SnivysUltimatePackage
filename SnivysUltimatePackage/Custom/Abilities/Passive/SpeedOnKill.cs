﻿using System.ComponentModel;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.Events.Handlers.Player;

namespace SnivysUltimatePackage.Custom.Abilities.Passive
{
    [CustomAbility]
    public class SpeedOnKill : PassiveAbility
    {
        public override string Name { get; set; } = "Speed on Kill";

        public override string Description { get; set; } = "Gives the user speed when they kill another player.";

        public float Duration { get; set; } = 5f;

        [Description("The highest intensity level of SCP-207 speed this ability can give.")]
        public byte IntensityLimit { get; set; } = 2;

        protected override void SubscribeEvents()
        {
            Player.Dying += OnDying;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.Dying -= OnDying;
            base.UnsubscribeEvents();
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (Check(ev.Attacker))
            {
                byte curIntensity = ev.Attacker.GetEffectIntensity<Scp207>();
                if (curIntensity < IntensityLimit)
                {
                    Log.Debug($"VVUP Custom Abilities: Applying Speed On Kill Effect to {ev.Attacker.Nickname}");
                    //ev.Attacker.ChangeEffectIntensity<Scp207>((byte)(curIntensity + 1));
                    //ev.Attacker.GetEffect(EffectType.Scp207).Duration = Duration;
                    ev.Attacker.EnableEffect(EffectType.Scp207, (byte)(curIntensity + 1), Duration);
                }
            }
        }
    }
}