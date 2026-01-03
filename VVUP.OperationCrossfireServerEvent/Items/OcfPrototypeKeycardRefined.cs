using System;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items.Keycards;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;

namespace VVUP.OperationCrossfireServerEvent.Items
{
    [CustomItem(ItemType.KeycardCustomManagement)]
    public class OcfPrototypeKeycardRefined : CustomItem, ICustomItemGlow
    {
        [YamlIgnore] 
        public override ItemType Type { get; set; } = ItemType.KeycardCustomManagement;
        public override uint Id { get; set; } = 45;
        public override string Name { get; set; } = "Prototype Keycard Refined";

        public override string Description { get; set; } =
            "This is the refined Prototype Keycard.";

        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; }
        
        public string KeycardName { get; set; } = "Prototype Keycard Refined";
        public string KeycardLabel { get; set; } = "Prototype Keycard Refined";

        [Description("The Containment Level of the Keycard (Max = 3)")]
        public int KeycardLevelContainment { get; set; } = 3;
        [Description("The Armory Level of the Keycard (Max = 3)")]
        public int KeycardLevelArmory { get; set; } = 0;
        [Description("The Admin Level of the Keycard (Max = 3)")]
        public int KeycardLevelAdmin { get; set; } = 2;
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
        public bool HasCustomItemGlow { get; set; } = false;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 255, 255, 255);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

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

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            base.OnAcquired(player, item, displayMessage);
            UpdateCard(item);
            if (OperationCrossfireEventHandlers.OcfStarted)
                OperationCrossfireEventHandlers._prototypeDeviceRefined = true;
        }

        public override Pickup Spawn(Vector3 position, Item item, Player previousOwner = null)
        {
            Pickup customKeyCard = Pickup.CreateAndSpawn(item.Type, position);
            UpdateCard(customKeyCard);
            return customKeyCard;
        }

        private void OnPickupAdded(PickupAddedEventArgs ev) 
        {
            if (!Check(ev.Pickup))
                return;

            UpdateCard(ev.Pickup); 
        }
        
        private void UpdateCard(Pickup pickup) //code taken from KeycardItem (kinda)
        {
            if (!Type.TryGetTemplate<KeycardItem>(out var item))
                throw new ArgumentException("Template for itemType not found");

            if (!item.Customizable)
                return;

            int num = 0;
            DetailBase[] details = item.Details;

            object[] args = new object[]
            {
                KeycardName,
                KeycardPermissions,
                KeycardPermissionsColor,
                KeycardPrimaryColor,
                KeycardLabel,
                KeycardLabelColor
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
        private void UpdateCard(Item item) //code taken from KeycardItem (kinda)
        {
            if (!Type.TryGetTemplate<KeycardItem>(out var keycardItem))
                throw new ArgumentException("Template for itemType not found");

            if (!keycardItem.Customizable)
                return;

            int num = 0;
            DetailBase[] details = keycardItem.Details;

            object[] args = new object[]
            {
                KeycardName,
                KeycardPermissions,
                KeycardPermissionsColor,
                KeycardPrimaryColor,
                KeycardLabel,
                KeycardLabelColor
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
    }
}