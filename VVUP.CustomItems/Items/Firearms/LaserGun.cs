using System.Collections.Generic;
using System.ComponentModel;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
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


        [Description("How long does the laser stay on the screen")]
        public float LaserVisibleTime { get; set; } = 0.5f;
        [Description("How big is the laser")]
        public Vector3 LaserScale { get; set; } = new Vector3(0.2f, 0.2f, 0.2f);
        protected override void OnShot(ShotEventArgs ev)
        {
            Log.Debug($"VVUP Custom Items: Laser Gun, spawning laser going from {ev.Player.Position} to {ev.Position}");
            var color = GetLaserColor(ev.Player);
            var laserColor = new Color(color.Red, color.Green, color.Blue);
            var direction = ev.Position - ev.Player.Position;
            var distance = direction.magnitude;
            var scale = new Vector3(LaserScale.x, distance * 0.5f, LaserScale.z);
            var laserPos = ev.Player.Position + direction * 0.5f;
            var rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
            Log.Debug($"VVUP Custom Items: Laser Gun, Laser Info: Position: {laserPos}, Rotation: {rotation.eulerAngles}, Color: {laserColor}");
            var laser = Primitive.Create(PrimitiveType.Cylinder, PrimitiveFlags.Visible, laserPos, rotation.eulerAngles,
                scale, true, laserColor);
            Timing.CallDelayed(LaserVisibleTime, laser.Destroy);
        }
        private (float Red, float Green, float Blue) GetLaserColor(Player player)
        {
            Log.Debug($"VVUP Custom Items: Getting laser color for {player.Nickname} from SSSS settings if they have one defined");
            float red, green, blue;
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunRedId, out var redSetting) 
                && int.TryParse(redSetting.SyncInputText, out int r) && r is > -1 and < 256)
            {
                red = r / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS red value: {r}");
            }
            else
            {
                red = LaserColorRed[Base.GetRandomNumber.GetRandomInt(LaserColorRed.Count)];
                Log.Debug($"VVUP Custom Items: Using random red value: {red}");
            }
            
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunGreenId, out var greenSetting)
                && int.TryParse(greenSetting.SyncInputText, out int g) && g is > -1 and < 256)
            {
                green = g / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS green value: {g}");
            }
            else
            {
                green = LaserColorGreen[Base.GetRandomNumber.GetRandomInt(LaserColorGreen.Count)];
                Log.Debug($"VVUP Custom Items: Using random green value: {green}");
            }
            
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunBlueId, out var blueSetting) 
                && int.TryParse(blueSetting.SyncInputText, out int b) && b is > -1 and < 256)
            {
                blue = b / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS blue value: {b}");
            }
            else
            {
                blue = LaserColorBlue[Base.GetRandomNumber.GetRandomInt(LaserColorBlue.Count)];
                Log.Debug($"VVUP Custom Items: Using random blue value: {blue}");
            }
    
            Log.Debug($"VVUP Custom Items: Final RGB values for {player.Nickname}: R:{red}, G:{green}, B:{blue}");
            return (red, green, blue);
        }
    }
}