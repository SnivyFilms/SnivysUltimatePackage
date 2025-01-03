﻿using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerStatsSystem;

namespace SnivysUltimatePackage.EventHandlers
{
    public class MicroDamageReductionEventHandler
    {
        public Plugin Plugin;
        public MicroDamageReductionEventHandler(Plugin plugin) => Plugin = plugin;

        public void OnPlayerHurting(HurtingEventArgs ev)
        {
            Log.Debug("VVUP Micro Damage Reduction: Checking if Micro Damage Reduction is enabled");
            if (!Plugin.Instance.Config.MicroDamageReductionConfig.IsEnabled)
                return;
            Log.Debug("VVUP Micro Damage Reduction: Checking if damage can be reduced");
            if (ev.Attacker != ev.Player && ev.Attacker != null && ev.Player != null && ev.DamageHandler.Type == DamageType.MicroHid &&
                Plugin.Instance.Config.MicroDamageReductionConfig.ScpDamageReduction.Contains(ev.Player.Role))
            {
                ev.Amount /= Plugin.Instance.Config.MicroDamageReductionConfig.ScpDamageReductionValue;
                Log.Debug($"VVUP Micro Damage Reduction: {ev.Player.Nickname} is {ev.Player.Role}, reducing damage");
            }
        }
    }
}