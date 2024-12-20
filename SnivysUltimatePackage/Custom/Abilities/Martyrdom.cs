﻿using System.ComponentModel;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using Item = Exiled.API.Features.Items.Item;

namespace SnivysUltimatePackage.Custom.Abilities
{
    [CustomAbility]
    public class Martyrdom : PassiveAbility
    {
        public override string Name { get; set; } = "Martyrdom";

        public override string Description { get; set; } = "Causes the player to explode upon death.";

        [Description("How long should the fuse be?")]
        public float ExplosiveFuse { get; set; } = 3f;

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
            if (Check(ev.Player))
            {
                ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                grenade.FuseTime = ExplosiveFuse;
                grenade.SpawnActive(ev.Player.Position);
            }
        }
    }
}