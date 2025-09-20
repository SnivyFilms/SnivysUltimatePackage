using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Extensions;
using MEC;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunE11SR)]
    public class LaserGun : CustomWeapon
    {
        public override ItemType Type { get; set; } = ItemType.GunE11SR;
        public override uint Id { get; set; } = 41;
        public override string Name { get; set; } = "<color=#FF0000>X-57 Helios Beam</color>";
        public override string Description { get; set; } = "It fires lasers!";
        public override float Weight { get; set; } = 2;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new ()
                {
                    Chance = 20,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
        };

        [Description("How long does the laser stay on the screen")]
        public float LaserVisibleTime { get; set; } = 0.03f;


        [Description("How bright is the laser?")]
        public float LaserBrightness { get; set; } = 300f;

        [Description("How big is the laser")]
        public Vector3 LaserScale { get; set; } = new Vector3(0.04f, 0, 0.04f);

        [Description("The red color of the laser, values must be between 0 and 1")]
        public List<float> LaserColorRed { get; set; } = new List<float>()
        {
            0.86f,
            1,
            0,
            0.55f,
            0.97f,
        };
        [Description("The green color of the laser, values must be between 0 and 1")]

        public List<float> LaserColorGreen { get; set; } = new List<float>()
        {
            0.08f,
            0.27f,
            0.84f,
            0.65f,
            0.5f,
            0.36f,
            0,
            0.97f,
        };

        [Description("The blue color of the laser, values must be between 0 and 1")]
        public List<float> LaserColorBlue { get; set; } = new List<float>()
        {
            0.24f,
            0,
            0.31f,
            1,
            0.96f,
        };

        protected override void OnShot(ShotEventArgs ev)
        {
            Vector3 origin;
            if (BarrelTipExtension.TryFindWorldmodelBarrelTip(ev.Player.CurrentItem.Serial, out BarrelTipExtension ext))
                origin = ext.WorldspacePosition;
            else
                origin = ev.Player.CameraTransform.position;

            Vector3 forward = ev.Position - origin;
            Vector3 laserSize = new Vector3(LaserScale.x, forward.magnitude * 0.5f, LaserScale.z);
            Vector3 laserPosition = origin + forward * 0.5f;

            Quaternion quaternion = Quaternion.LookRotation(forward) * Quaternion.Euler(90f, 0.0f, 0.0f);
            Color color = GetLaserColor(ev.Player);
            Primitive laser = Primitive.Create(
                PrimitiveType.Cylinder,
                (PrimitiveFlags)2,
                new Vector3?(laserPosition),
                new Vector3?(quaternion.eulerAngles),
                new Vector3?(laserSize),
                true,
                new Color?(color)
            );

            Timing.RunCoroutine(LaserFade(laser));
        }

        private Color GetLaserColor(Player player)
        {
            Color color = new Color();
            Log.Debug($"VVUP Custom Items: Getting laser color for {player.Nickname} from SSSS settings if they have one defined");
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunRedId, out var redSetting) 
                && int.TryParse(redSetting.SyncInputText, out int r) && r is > -1 and < 256)
            {
                color.r = r / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS red value: {r}");
            }
            else
            {
                color.r = LaserColorRed[Base.GetRandomNumber.GetRandomInt(LaserColorRed.Count)];
                Log.Debug($"VVUP Custom Items: Using random red value: {color.r}");
            }
            
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunGreenId, out var greenSetting)
                && int.TryParse(greenSetting.SyncInputText, out int g) && g is > -1 and < 256)
            {
                color.g = g / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS green value: {g}");
            }
            else
            {
                color.g = LaserColorGreen[Base.GetRandomNumber.GetRandomInt(LaserColorGreen.Count)];
                Log.Debug($"VVUP Custom Items: Using random green value: {color.g}");
            }
            
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunBlueId, out var blueSetting) 
                && int.TryParse(blueSetting.SyncInputText, out int b) && b is > -1 and < 256)
            {
                color.b = b / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS blue value: {b}");
            }
            else
            {
                color.b = LaserColorBlue[Base.GetRandomNumber.GetRandomInt(LaserColorBlue.Count)];
                Log.Debug($"VVUP Custom Items: Using random blue value: {color.b}");
            }
            
            Color.RGBToHSV(color, out float h, out float s, out float v);
            Color finalColor = Color.HSVToRGB(h, 1f, 1f);

            Log.Debug($"VVUP Custom Items: Final RGB values for {player.Nickname}: R:{finalColor.r}, G:{finalColor.g}, B:{finalColor.b}");

            return finalColor;
        }
        IEnumerator<float> LaserFade(Primitive Laser)
        {
            Color color = new Color(Laser.Color.r, Laser.Color.g, Laser.Color.b, 1f);

            for (int i = 20; i > 0; i--)
            {
                float brightness = (LaserBrightness * (i / 20f));
                Color newColor = new Color(color.r * brightness, color.g * brightness, color.b * brightness, 1f);

                Laser.Color = newColor;

                yield return Timing.WaitForSeconds(0.05f / 20f);
            }

            Laser.Destroy();
        }
    }
}