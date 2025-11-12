using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;
using VVUP.Base.API;
using VVUP.CustomItems.API;
using YamlDotNet.Serialization;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class F4 : CustomGrenade, ICustomItemGlow
    {
        public enum F4RemoveMethod
        {
            Remove = 0,
            Detonate = 1,
            Drop = 2,
        }
        public static F4 Instance { get; private set; } = null!;
        public static Dictionary<Pickup, Player> PlacedCharges { get; } = new();
        public override uint Id { get; set; } = 54;
        public override string Name { get; set; } = "<color=#6600CC>F4</color>";
        public override float Weight { get; set; } = 0.75f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideLczArmory,
                },

                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHczArmory,
                },

                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside049Armory,
                },
                
                new ()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside079Armory,
                },
                
                new()
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideSurfaceNuke,
                },
            },
        };
        public override string Description { get; set; } = "Flashbang that can be remotely detonated.";
        
        [Description("Should F4 require a specific item to be detonated.")]
        public bool RequireDetonator { get; set; } = true;
        
        [Description("The Detonator Item that will be used to detonate F4 Charges")]
        public ItemType DetonatorItem { get; set; } = ItemType.Radio;
        
        [Description(
            "What happens with F4 charges placed by player, when he dies/leaves the game. (Remove / Detonate / Drop)")]
        public F4RemoveMethod MethodOnDeath { get; set; } = F4RemoveMethod.Drop;
        [Description("Should shooting at F4 charges do something.")]
        public bool AllowShoot { get; set; } = true;
        
        [Description("What happens with F4 charges after they are shot. (Remove / Detonate / Drop)")]
        public F4RemoveMethod ShotMethod { get; set; } = F4RemoveMethod.Remove;
        
        [Description("Maximum distance between F4 Charge and player to detonate.")]
        public float MaxDistance { get; set; } = 100f;
        
        [Description("Time after which the F4 charge will automatically detonate.")]
        public override float FuseTime { get; set; } = 9999f;
        
        [Description("Will F4 explosion be associated with the player who deployed it or the server")]
        public bool AssociateF4WithServer { get; set; } = false;
        [Description("Determines if the F4 will stick to surfaces or it rolls on the ground.")]
        public bool Sticky { get; set; } = true;
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        
        [YamlIgnore]
        public override bool ExplodeOnCollision { get; set; } = false;
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public void F4Handler(Pickup? charge, F4RemoveMethod removeMethod = F4RemoveMethod.Detonate, Player? player = null)
        {
            if (charge?.Base?.gameObject == null)
                return;

            if (player == null && PlacedCharges.TryGetValue(charge, out var placedPlayer))
            {
                player = placedPlayer;
            }

            switch (removeMethod)
            {
                case F4RemoveMethod.Remove:
                {
                    break;
                }

                case F4RemoveMethod.Detonate:
                {
                    FlashGrenade grenade = (FlashGrenade)Item.Create(Type);
                    grenade.FuseTime = 0.1f;
                    grenade.SpawnActive(charge.Position, owner: AssociateF4WithServer ? Server.Host : player);
                    break;
                }

                case F4RemoveMethod.Drop:
                {
                    TrySpawn(Id, charge.Position, out _);
                    break;
                }
            }

            PlacedCharges.Remove(charge);
            charge.Destroy();
        }
        
        protected override void SubscribeEvents()
        {
            Instance = this;

            StickyGrenadeApi.RegisterStickyGrenade("F4", pickup => Sticky && PlacedCharges.ContainsKey(pickup));
            
            PlayerEvent.Destroying += OnDestroying;
            PlayerEvent.Died += OnDied;
            PlayerEvent.Shooting += OnShooting;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            base.SubscribeEvents();
        }
        
        protected override void UnsubscribeEvents()
        {
            StickyGrenadeApi.UnregisterStickyGrenade("F4");
            PlayerEvent.Destroying -= OnDestroying;
            PlayerEvent.Died -= OnDied;
            PlayerEvent.Shooting -= OnShooting;

            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            PlacedCharges.Clear();

            base.OnWaitingForPlayers();
        }

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (!PlacedCharges.ContainsKey(ev.Projectile))
                PlacedCharges.Add(ev.Projectile, ev.Player);
            
            base.OnThrownProjectile(ev);
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            if (!AssociateF4WithServer)
                ev.Projectile.PreviousOwner = ev.Player;
            Pickup pickup = Pickup.Get(ev.Projectile.Base);
            PlacedCharges.Remove(pickup);
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            foreach (var charge in PlacedCharges.ToList())
            {
                if (charge.Value == ev.Player)
                {
                    F4Handler(charge.Key, F4RemoveMethod.Remove, ev.Player);
                }
            }
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (ev.Player == null)
                return;

            foreach (var charge in PlacedCharges.ToList().Where(charge => charge.Value == ev.Player))
            {
                F4Handler(charge.Key, MethodOnDeath, ev.Player);
            }
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (!AllowShoot)
                return;

            Vector3 forward = ev.Player.CameraTransform.forward;
            if (Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, 500))
            {
                EffectGrenade grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();
                if (grenade == null)
                {
                    return;
                }

                if (PlacedCharges.ContainsKey(Pickup.Get(grenade)))
                {
                    F4Handler(Pickup.Get(grenade), ShotMethod, ev.Player);
                }
            }
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            PlacedCharges.Clear();
            StickyGrenadeApi.ClearTrackedGrenades();
        }
    }
}