using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Item = Exiled.API.Features.Items.Item;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class Martyrdom : PassiveAbility
    {
        public override string Name { get; set; } = "Martyrdom";

        public override string Description { get; set; } = "Causes the player to explode upon death.";

        [Description("How long should the fuse be?")]
        public float ExplosiveFuse { get; set; } = 3f;

        [Description("Should the explosive from player death be related to the player or the server")]
        public bool ShouldServerBeKiller { get; set; } = false;

        protected override void AbilityAdded(Exiled.API.Features.Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Explode on Death, Adding Explode on Death to {player.Nickname}");
            Player.Dying += OnDying;
            base.AbilityAdded(player);
        }
        protected override void AbilityRemoved(Exiled.API.Features.Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Explode on Death, Removing Explode on Death from {player.Nickname}");
            Player.Dying -= OnDying;
            base.AbilityRemoved(player);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (Check(ev.Player))
            {
                Log.Debug($"VVUP Custom Abilities: Spawning Grenade at {ev.Player.Nickname} death location");
                ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                grenade.FuseTime = ExplosiveFuse;
                if (ShouldServerBeKiller)
                    grenade.SpawnActive(ev.Player.Position, Server.Host);
                else
                {
                    grenade.ChangeItemOwner(Server.Host, ev.Player);
                    grenade.SpawnActive(ev.Player.Position);
                }
            }
        }
    }
}