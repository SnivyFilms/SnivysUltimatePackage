using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using NorthwoodLib.Pools;
using TMPro;
using UnityEngine;
using UserSettings.ServerSpecific;
using VVUP.CustomItems.Items.Armor;
using VVUP.CustomItems.Items.Firearms;
using VVUP.CustomItems.Items.Grenades;
using VVUP.CustomItems.Items.MedicalItems;
using VVUP.CustomItems.Items.Other;

namespace VVUP.CustomItems
{
    public class SsssHelper
    {
        public static ServerSpecificSettingBase[] GetSettings()
        {
            List<ServerSpecificSettingBase> settings = new List<ServerSpecificSettingBase>();
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            //settings.Add(new SSGroupHeader("Vicious Vikki's Custom Items"));
            var customItems = new List<IEnumerable<CustomItem>>
            {
                ExplosiveResistantArmor.Get(typeof(ExplosiveResistantArmor)),
                ExplosiveRoundRevolver.Get(typeof(ExplosiveRoundRevolver)),
                MediGun.Get(typeof(MediGun)),
                Tranquilizer.Get(typeof(Tranquilizer)),
                C4.Get(typeof(C4)),
                EmpGrenade.Get(typeof(EmpGrenade)),
                NerveAgentGrenade.Get(typeof(NerveAgentGrenade)),
                SmokeGrenade.Get(typeof(SmokeGrenade)),
                DeadringerSyringe.Get(typeof(DeadringerSyringe)),
                KySyringe.Get(typeof(KySyringe)),
                AntiScp096Pills.Get(typeof(AntiScp096Pills)),
                PhantomLantern.Get(typeof(PhantomLantern)),
                Scp1499.Get(typeof(Scp1499)),
                InfinitePills.Get(typeof(InfinitePills)),
                ClusterGrenade.Get(typeof(ClusterGrenade)),
                AdditionalHealth207.Get(typeof(AdditionalHealth207)),
                LowGravityArmor.Get(typeof(LowGravityArmor)),
                ViperPdw.Get(typeof(ViperPdw)),
                Pathfinder.Get(typeof(Pathfinder)),
                LaserGun.Get(typeof(LaserGun)),
                MultiFlash.Get(typeof(MultiFlash)),
                ProxyBang.Get(typeof(ProxyBang)),
                GrenadeLauncher.Get(typeof(GrenadeLauncher)),
                PortableIntercom.Get(typeof(PortableIntercom)),
                Telewand.Get(typeof(Telewand)),
                LowGravityGrenade.Get(typeof(LowGravityGrenade)),
                CognitohazardCharm.Get(typeof(CognitohazardCharm)),
                BottleOfRum.Get(typeof(BottleOfRum)),
                F4.Get(typeof(F4)),
            };

            foreach (var itemCollection in customItems)
            {
                if (itemCollection == null) continue;

                foreach (var items in itemCollection)
                {
                    stringBuilder.AppendLine($"Item: {items.Name}");
                    stringBuilder.AppendLine($"- Description: {items.Description}");
                }
                    
            }
            settings.Add(new SSTextArea(Plugin.Instance.Config.SsssConfig.CustomItemTextId, StringBuilderPool.Shared.ToStringReturn(stringBuilder),
                SSTextArea.FoldoutMode.CollapsedByDefault));
            stringBuilder.Clear();
            
            settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.DetonateC4Id, Plugin.Instance.Config.SsssConfig.DetonateC4SsssText,
                KeyCode.J, true, false, Plugin.Instance.Config.SsssConfig.DetonateC4Hint));
            
            settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.GrenadeLauncherForceModeId,Plugin.Instance.Config.SsssConfig.GrenadeLauncherForceModeSsssText,
                KeyCode.K, true, false, Plugin.Instance.Config.SsssConfig.GrenadeLauncherForceModeHint));
    
            settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.GrenadeLauncherLaunchModeId,Plugin.Instance.Config.SsssConfig.GrenadeLauncherLaunchModeSsssText,
                KeyCode.L, true, false, Plugin.Instance.Config.SsssConfig.GrenadeLauncherLaunchModeHint));
            
            settings.Add(new SSPlaintextSetting(
                Plugin.Instance.Config.SsssConfig.LaserGunRedId,
                $"{Plugin.Instance.Config.SsssConfig.LaserGunColorText} {Plugin.Instance.Config.SsssConfig.LaserGunColorTextRed}",
                "-2",
                3,
                TMP_InputField.ContentType.IntegerNumber,
                Plugin.Instance.Config.SsssConfig.LaserGunColorHint));

            settings.Add(new SSPlaintextSetting(
                Plugin.Instance.Config.SsssConfig.LaserGunGreenId,
                $"{Plugin.Instance.Config.SsssConfig.LaserGunColorText} {Plugin.Instance.Config.SsssConfig.LaserGunColorTextGreen}",
                "-2",
                3,
                TMP_InputField.ContentType.IntegerNumber,
                Plugin.Instance.Config.SsssConfig.LaserGunColorHint));

            settings.Add(new SSPlaintextSetting(
                Plugin.Instance.Config.SsssConfig.LaserGunBlueId,
                $"{Plugin.Instance.Config.SsssConfig.LaserGunColorText} {Plugin.Instance.Config.SsssConfig.LaserGunColorTextBlue}",
                "-2",
                3,
                TMP_InputField.ContentType.IntegerNumber,
                Plugin.Instance.Config.SsssConfig.LaserGunColorHint));
            return settings.ToArray();
        }
        public static void SafeAppendSsssSettings()
        {
            var mySettings = GetSettings();
            var current = ServerSpecificSettingsSync.DefinedSettings?.ToList() ?? new List<ServerSpecificSettingBase>();
            bool needToAddSettings = false;
            foreach (var setting in mySettings)
            {
                if (current.All(s => s.SettingId != setting.SettingId))
                {
                    needToAddSettings = true;
                    break;
                }
            }
            if (needToAddSettings)
            {
                if (!current.Any(s => s is SSGroupHeader header && header.Label == Plugin.Instance.Config.SsssConfig.Header))
                {
                    current.Add(new SSGroupHeader(Plugin.Instance.Config.SsssConfig.Header));
                }
                foreach (var setting in mySettings)
                {
                    if (current.All(s => s.SettingId != setting.SettingId))
                        current.Add(setting);
                    else
                        Log.Debug($"VVUP CI SSSS: Skipped duplicate SettingId: {setting.SettingId}");
                }
        
                ServerSpecificSettingsSync.DefinedSettings = current.ToArray();
                Log.Debug($"VVUP CI SSSS: Appended settings. Total now: {current.Count}");
            }
        }
    }
}