﻿using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace SnivysUltimatePackage.Custom.Abilities.Passive
{
    [CustomAbility]
    public class LifeSteal : PassiveAbility
    {
        public override string Name { get; set; } = "Life Steal";

        public override string Description { get; set; } =
            "When dealing damage to a player, you are able to heal a set percentage of the damage dealt.";
        
        public List<Player> PlayersWithLifeSteal = new List<Player>();
        public float LifeStealPercentage { get; set; } = 0.1f;
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: LifeSteal, Adding LifeSteal Ability to {player.Nickname}");
            PlayersWithLifeSteal.Add(player);
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }
        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: LifeSteal, Removing LifeSteal Ability from {player.Nickname}");
            PlayersWithLifeSteal.Remove(player);
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }
        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null)
                return;
            
            if (!PlayersWithLifeSteal.Contains(ev.Attacker))
                return;
            
            if (ev.Attacker.IsAlive && ev.Player.IsAlive && ev.Attacker != ev.Player)
            {
                ev.Attacker.Heal(ev.Amount * LifeStealPercentage);
                Log.Debug($"VVUP Custom Abilities: LifeSteal, {ev.Attacker.Nickname} healed for {ev.Amount * LifeStealPercentage} health from {ev.Player.Nickname}");
            }
        }
    }
}