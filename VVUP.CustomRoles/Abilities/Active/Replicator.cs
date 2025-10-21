using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class Replicator : ActiveAbility
    {
        public override string Name { get; set; } = "Replicator";
        public override string Description { get; set; } = "Create Decoy and make Recon";
        public override float Duration { get; set; } = 15f;
        public override float Cooldown { get; set; } = 45f;
        public string EndedMessage { get; set; } = "<color=red>Replicator Expired</color>";
        public string KilledDecoy { get; set; } = "Your decoy has been killed!";

        private readonly Dictionary<Player, Npc> decoys = new();
        private readonly Dictionary<Player, Vector3> startPos = new();

        protected override void AbilityUsed(Player player)
        {
            startPos[player] = player.Position;

            Npc dummy = Npc.Spawn(player.Nickname, player.Role.Type, player.Position);
            decoys[player] = dummy;

            Exiled.Events.Handlers.Player.Hurting += OnPlayerHurting;
            Exiled.Events.Handlers.Player.Shooting += OnPlayerShooting;
            Exiled.Events.Handlers.Player.UsingItem += OnPlayerUsingItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnPlayerDropping;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPlayerPickingUp;
            Exiled.Events.Handlers.Player.Dying += OnDummyDying;
        }

        protected override void AbilityEnded(Player player)
        {
            if (decoys.TryGetValue(player, out Npc dummy))
            {
                player.Position = dummy.Position;
                dummy.Destroy();
            }

            decoys.Remove(player);
            startPos.Remove(player);

            Exiled.Events.Handlers.Player.Hurting -= OnPlayerHurting;
            Exiled.Events.Handlers.Player.Shooting -= OnPlayerShooting;
            Exiled.Events.Handlers.Player.DroppingItem -= OnPlayerDropping;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPlayerPickingUp;
            Exiled.Events.Handlers.Player.UsingItem -= OnPlayerUsingItem;
            Exiled.Events.Handlers.Player.Dying -= OnDummyDying;
        }

        private void OnPlayerHurting(HurtingEventArgs ev)
        {
            if (decoys.ContainsKey(ev.Player))
            {
                ev.IsAllowed = false;
                EndAbility(ev.Player);
            }
        }

        private void OnPlayerShooting(ShootingEventArgs ev)
        {
            if (decoys.ContainsKey(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnPlayerUsingItem(UsingItemEventArgs ev)
        {
            if (decoys.ContainsKey(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnPlayerDropping(DroppingItemEventArgs ev)
        {
            if (decoys.ContainsKey(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnPlayerPickingUp(PickingUpItemEventArgs ev)
        {
            if (decoys.ContainsKey(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnDummyDying(DyingEventArgs ev)
        {
            foreach (var kvp in decoys.Where(kvp => kvp.Value != null && kvp.Value.ReferenceHub == ev.Player.ReferenceHub))
            {
                kvp.Key.Kill(KilledDecoy);
                EndAbility(kvp.Key);
                break;
            }
        }
    }
}
