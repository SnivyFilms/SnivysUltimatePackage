using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using YamlDotNet.Serialization;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.MedicalItems
{
    [CustomItem(ItemType.Adrenaline)]
    public class KySyringe : CustomItem
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.Adrenaline;
        private bool KillAfterAnimation { get; set; } = true;
        private string KillReason { get; set; } = "Intentional Fatal Injection";
        public override uint Id { get; set; } = 26;
        public override string Name { get; set; } = "<color=#0000CC>LJ-429</color>";
        public override string Description { get; set; } = "When injected, the user has a quick death.";
        public override float Weight { get; set; } = 1.15f;
        [Description("Removes the Syringe on use, otherwise it just drops on the floor after the player dies (it's really funny ngl)")]
        public bool RemoveSyringeOnUse { get; set; } = true;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside096,
                },
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = SpawnLocationType.Inside939Cryo,
                },
            }
        };
        protected override void SubscribeEvents()
        {
            if (KillAfterAnimation)
                Player.UsingItemCompleted += OnUsingLJAnimation;
            else
                Player.UsingItem += OnUsingLJ;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            if (KillAfterAnimation)
                Player.UsingItemCompleted -= OnUsingLJAnimation;
            else
                Player.UsingItem -= OnUsingLJ;

            base.UnsubscribeEvents();
        }
        private void OnUsingLJ(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Log.Debug($"VVUP Custom Items: KY Syringe, Killing {ev.Player.Nickname}");
            if (RemoveSyringeOnUse)
                ev.Player.RemoveItem(ev.Item);
            ev.Player.Kill(KillReason);
            ev.Player.Health = 1f;
            ev.Player.EnableEffect(EffectType.Bleeding, 500f);
            ev.Player.EnableEffect(EffectType.Corroding, 500f);
        }

        private void OnUsingLJAnimation(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Log.Debug($"VVUP Custom Items: KY Syringe, Killing {ev.Player.Nickname}");
            if (RemoveSyringeOnUse)
                ev.Player.RemoveItem(ev.Item);
            ev.Player.Kill(KillReason);
            ev.Player.Health = 1f;
            ev.Player.EnableEffect(EffectType.Bleeding, 500f);
            ev.Player.EnableEffect(EffectType.Corroding, 500f);
        }
    }
}