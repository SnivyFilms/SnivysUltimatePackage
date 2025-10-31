using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using PlayerRoles;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using LabKeycardItem = LabApi.Features.Wrappers.KeycardItem;

namespace VVUP.OperationCrossfireServerEvent.Items
{
    [CustomItem(ItemType.KeycardCustomSite02)]
    public class OcfPrototypeKeycardBasic : CustomItem, ICustomItemGlow
    {
        [YamlIgnore] 
        public override ItemType Type { get; set; } = ItemType.KeycardCustomSite02;
        public override uint Id { get; set; } = 44;
        public override string Name { get; set; } = "Prototype Keycard Basic";

        public override string Description { get; set; } =
            "This is the basic Prototype Keycard needed to unlock SCP-914";

        public override float Weight { get; set; } = 0.5f;
        
        public string KeycardName { get; set; } = "Prototype Keycard Basic";
        public string KeycardLabel { get; set; } = "Prototype Keycard Basic";
        [Description("The Holder of the Keycard (Leave blank for random)")]
        public string KeycardHolder { get; set; } = "";
        [Description("The Wear level of the Keycard (0-255)")]
        public byte KeycardWearLevel { get; set; } = 0;

        [Description("The Containment Level of the Keycard (Max = 3)")]
        public int KeycardLevelContainment { get; set; } = 1;
        [Description("The Armory Level of the Keycard (Max = 3)")]
        public int KeycardLevelArmory { get; set; } = 0;
        [Description("The Admin Level of the Keycard (Max = 3)")]
        public int KeycardLevelAdmin { get; set; } = 0;
        [YamlIgnore]
        public KeycardLevels KeycardPermissions => new KeycardLevels(KeycardLevelContainment, KeycardLevelArmory, KeycardLevelAdmin);
        [Description("Primary Color Red of the Keycard (0-255)")]
        public byte KeycardPrimaryColorRed { get; set; } = 255;
        [Description("Primary Color Green of the Keycard (0-255)")]
        public byte KeycardPrimaryColorGreen { get; set; } = 0;
        [Description("Primary Color Blue of the Keycard (0-255)")]
        public byte KeycardPrimaryColorBlue { get; set; } = 255;
        [Description("Primary Color Brightness of the Keycard (0-255)")]
        public byte KeycardPrimaryColorAlpha { get; set; } = 255;
        
        [YamlIgnore]
        public Color32 KeycardPrimaryColor => new Color32(KeycardPrimaryColorRed, KeycardPrimaryColorGreen, KeycardPrimaryColorBlue, KeycardPrimaryColorAlpha);
        [Description("Label Color Red of the Keycard (0-255)")]
        public byte KeycardLabelColorRed { get; set; } = 255;
        [Description("Green Color Red of the Keycard (0-255)")]
        public byte KeycardLabelColorGreen { get; set; } = 255;
        [Description("Blue Color Red of the Keycard (0-255)")]
        public byte KeycardLabelColorBlue { get; set; } = 255;
        [Description("Color Brightness of the Keycard's Label (0-255)")]
        public byte KeycardLabelColorAlpha { get; set; } = 255;
        [YamlIgnore]
        public Color32 KeycardLabelColor => new Color32(KeycardLabelColorRed, KeycardLabelColorGreen, KeycardLabelColorBlue, KeycardLabelColorAlpha);
        [Description("Permissions Color Red of the Keycard (0-255)")]
        public byte KeycardPermissionColorRed { get; set; } = 0;
        [Description("Permissions Color Green of the Keycard (0-255)")]
        public byte KeycardPermissionColorGreen { get; set; } = 0;
        [Description("Permissions Color Blue of the Keycard (0-255)")]
        public byte KeycardPermissionColorBlue { get; set; } = 0;
        [Description("Permissions Color Brightness of the Keycard (0-255)")]
        public byte KeycardPermissionColorAlpha { get; set; } = 255;
        [YamlIgnore]
        public Color32 KeycardPermissionsColor => new Color32(KeycardPermissionColorRed, KeycardPermissionColorGreen, KeycardPermissionColorBlue, KeycardPermissionColorAlpha);
        
        public uint RefinedKeycardId { get; set; } = 45;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new()
                {
                    Chance = 0,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 0,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 0,
                    Location = SpawnLocationType.InsideGr18Glass,
                },
                new()
                {
                    Chance = 0,
                    Location = SpawnLocationType.Inside106Primary,
                },
                new()
                {
                    Chance = 0,
                    Location = SpawnLocationType.Inside330,
                },
                new()
                {
                    Chance = 0,
                    Location = SpawnLocationType.Inside173Gate,
                },
            }
        };

        public bool HasCustomItemGlow { get; set; } = false;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 255, 255, 255);
        public float GlowRange { get; set; } = 0.25f;
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Map.PickupAdded += OnPickupAdded;
        }
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Map.PickupAdded -= OnPickupAdded;
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.ClassD && OperationCrossfireEventHandlers.OcfStarted)
                ev.IsAllowed = false;
            base.OnDroppingItem(ev);
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            base.OnAcquired(player, item, displayMessage);
            UpdateCard(item);
        }

        public override Pickup Spawn(Vector3 position, Item item, Player previousOwner = null)
        {
            Pickup customKeyCard = Pickup.CreateAndSpawn(item.Type, position);
            UpdateCard(customKeyCard);

            if (!TrackedSerials.Contains(item.Serial))
            {
                TrackedSerials.Add(item.Serial);
            }

            return customKeyCard;
        }

        private void OnPickupAdded(PickupAddedEventArgs ev) 
        {
            if (!Check(ev.Pickup))
                return;

            UpdateCard(ev.Pickup); 
        }
        
        private void UpdateCard(Pickup pickup) //just realized that these methods used to update it for EVERY card, not one specific card
        {
            LabKeycardItem card = (LabKeycardItem) LabKeycardItem.Get(pickup.Serial); //the new one doesnt even work so we might just need to wait for a labapi update or ask whoever made it for an edit card info method
            if (card == null)
            {
                Log.Warn($"Custom keycard with serial {pickup.Serial} does not exist.");
                return;
            }
            if (!card.Base.Customizable)
            {
                Log.Warn($"Custom keycard with serial {pickup.Serial} is not customizable.");
                return;
            }

            int num = 0;
            DetailBase[] details = card.Base.Details;

            object[] args = new object[] //KeycardCustomSite02 has different arguments for customization than KeycardCustomManagement
            {
                KeycardName,                // itemName
                KeycardPermissions,         // permissions
                KeycardPermissionsColor,    // (Color32)permissionsColor
                KeycardPrimaryColor,        // (Color32)keycardColor
                KeycardLabel,               // cardLabel
                KeycardLabelColor,          // (Color32)labelColor
                KeycardHolder,              // holderName
                KeycardWearLevel 
            };

            for (int i = 0; i < details.Length; i++)
            {
                if (details[i] is ICustomizableDetail customizableDetail)
                {
                    customizableDetail.SetArguments(new ArraySegment<object>(args, num, customizableDetail.CustomizablePropertiesAmount));
                    num += customizableDetail.CustomizablePropertiesAmount;
                }
            }
        }
        private void UpdateCard(Item item)
        {
            LabKeycardItem card = (LabKeycardItem) LabKeycardItem.Get(item.Serial);

            if (card == null)
            {
                Log.Warn($"Custom keycard with serial {item.Serial} does not exist.");
                return;
            }

            if (!card.Base.Customizable)
            {
                Log.Warn($"Custom keycard with serial {item.Serial} is not customizable.");
                return;
            }

            int num = 0;
            DetailBase[] details = card.Base.Details;

            object[] args = new object[]
            {
                KeycardName,
                KeycardHolder,
                KeycardName,
                KeycardPermissions,
                KeycardPrimaryColor,
                KeycardPermissionsColor,
                KeycardLabelColor,
                KeycardWearLevel
            };

            for (int i = 0; i < details.Length; i++)
            {
                if (details[i] is ICustomizableDetail customizableDetail)
                {
                    customizableDetail.SetArguments(new ArraySegment<object>(args, num, customizableDetail.CustomizablePropertiesAmount));
                    num += customizableDetail.CustomizablePropertiesAmount;
                }
            }
        }
        protected override void OnUpgrading(UpgradingEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Item.DestroySelf();
            if (OperationCrossfireEventHandlers.OcfStarted)
                TrySpawn(RefinedKeycardId, ev.OutputPosition, out var pickup);
        }
    }
}