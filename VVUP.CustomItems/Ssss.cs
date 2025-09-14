using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using NorthwoodLib.Pools;
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
            settings.Add(new SSTextArea(Plugin.Instance.Config.CustomItemTextId, StringBuilderPool.Shared.ToStringReturn(stringBuilder),
                SSTextArea.FoldoutMode.CollapsedByDefault));
            stringBuilder.Clear();
            
            settings.Add(new SSKeybindSetting(Plugin.Instance.Config.DetonateC4Id, Plugin.Instance.Config.DetonateC4SsssText,
                KeyCode.J, true, false, Plugin.Instance.Config.DetonateC4Hint));
            
            settings.Add(new SSKeybindSetting(Plugin.Instance.Config.GrenadeLauncherForceModeId,Plugin.Instance.Config.GrenadeLauncherForceModeSsssText,
                KeyCode.K, true, false, Plugin.Instance.Config.GrenadeLauncherForceModeHint));
    
            settings.Add(new SSKeybindSetting(Plugin.Instance.Config.GrenadeLauncherLaunchModeId,Plugin.Instance.Config.GrenadeLauncherLaunchModeSsssText,
                KeyCode.K, true, false, Plugin.Instance.Config.GrenadeLauncherLaunchModeHint));
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
                if (!current.Any(s => s is SSGroupHeader header && header.Label == Plugin.Instance.Config.Header))
                {
                    current.Add(new SSGroupHeader(Plugin.Instance.Config.Header));
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