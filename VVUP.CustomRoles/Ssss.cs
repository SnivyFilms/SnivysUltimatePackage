using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using NorthwoodLib.Pools;
using UnityEngine;
using UserSettings.ServerSpecific;
using VVUP.CustomRoles.Roles.Chaos;
using VVUP.CustomRoles.Roles.ClassD;
using VVUP.CustomRoles.Roles.Foundation;
using VVUP.CustomRoles.Roles.Other;
using VVUP.CustomRoles.Roles.Scientist;
using VVUP.CustomRoles.Roles.Scps;

namespace VVUP.CustomRoles
{
    public class SsssHelper
    {
        public static ServerSpecificSettingBase[] GetSettings()
        {
            List<ServerSpecificSettingBase> settings = new List<ServerSpecificSettingBase>();
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            //settings.Add(new SSGroupHeader("Vicious Vikki's Custom Roles"));
            var customRoles = new List<CustomRole>
            {
                A7Chaos.Get(typeof(A7Chaos)),
                Biochemist.Get(typeof(Biochemist)),
                BorderPatrol.Get(typeof(BorderPatrol)),
                CiPhantom.Get(typeof(CiPhantom)),
                CISpy.Get(typeof(CISpy)),
                ClassDAnalyst.Get(typeof(ClassDAnalyst)),
                ClassDTank.Get(typeof(ClassDTank)),
                ContainmentGuard.Get(typeof(ContainmentGuard)),
                ContainmentScientist.Get(typeof(ContainmentScientist)),
                Demolitionist.Get(typeof(Demolitionist)),
                DwarfZombie.Get(typeof(DwarfZombie)),
                ExplosiveZombie.Get(typeof(ExplosiveZombie)),
                Flipped.Get(typeof(Flipped)),
                InfectedZombie.Get(typeof(InfectedZombie)),
                JuggernautChaos.Get(typeof(JuggernautChaos)),
                LockpickingClassD.Get(typeof(LockpickingClassD)),
                MedicZombie.Get(typeof(MedicZombie)),
                MtfParamedic.Get(typeof(MtfParamedic)),
                MtfWisp.Get(typeof(MtfWisp)),
                Nightfall.Get(typeof(Nightfall)),
                PoisonousZombie.Get(typeof(PoisonousZombie)),
                SpeedsterZombie.Get(typeof(SpeedsterZombie)),
                TelepathicChaos.Get(typeof(TelepathicChaos)),
                TeleportZombie.Get(typeof(TeleportZombie)),
                TheoreticalPhysicistScientist.Get(typeof(TheoreticalPhysicistScientist)),
                Vanguard.Get(typeof(Vanguard)),
                SoundBreaker173.Get(typeof(SoundBreaker173)),
                Replicant.Get(typeof(Replicant)),
                Dwarf.Get(typeof(Dwarf)),
                PaperMan.Get(typeof(PaperMan)),
                QuantumGravityResearcher.Get(typeof(QuantumGravityResearcher)),
            };
                
            foreach (var role in customRoles)
            {
                if (role == null || role.CustomAbilities == null) continue;

                stringBuilder.AppendLine($"Role: {role.Name}");
                stringBuilder.AppendLine($"- Description: {role.Description}");
                foreach (var ability in role.CustomAbilities)
                {
                    stringBuilder.AppendLine($"-- Ability: {ability.Name}, {ability.Description}");
                }
            }
            settings.Add(new SSTextArea(Plugin.Instance.Config.SsssConfig.CustomRoleTextId, StringBuilderPool.Shared.ToStringReturn(stringBuilder),
                SSTextArea.FoldoutMode.CollapsedByDefault));
            stringBuilder.Clear();
            
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.ActiveCamoId, Plugin.Instance.Config.SsssConfig.ActiveCamoSsssText, KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.ActiveCamoHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.ChargeId, Plugin.Instance.Config.SsssConfig.ChargeSsssText, KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.ChargeHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.DetectId, Plugin.Instance.Config.SsssConfig.DetectSsssText, KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.DetectHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.DoorPickingId, Plugin.Instance.Config.SsssConfig.DoorPickingSsssText, KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.DoorPickingHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.HealingMistId, Plugin.Instance.Config.SsssConfig.HealingMistSsssText, KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.HealingMistHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.RemoveDisguiseId, Plugin.Instance.Config.SsssConfig.RemoveDisguiseSsssText, KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.RemoveDisguiseHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.ReviveMistId, Plugin.Instance.Config.SsssConfig.ReviveMistSsssText, KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.ReviveMistHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.TeleportId, Plugin.Instance.Config.SsssConfig.TeleportSsssText, KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.TeleportHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.SoundBreakerId, Plugin.Instance.Config.SsssConfig.SoundBreakerSsssText,KeyCode.C, true, false, Plugin.Instance.Config.SsssConfig.SoundBreakerHint));
             settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.ReplicatorId, Plugin.Instance.Config.SsssConfig.ReplicatorSsssText,KeyCode.B, true, false, Plugin.Instance.Config.SsssConfig.ReplicatorHint));
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
                        Log.Debug($"VVUP CR SSSS: Skipped duplicate SettingId: {setting.SettingId}");
                }
        
                ServerSpecificSettingsSync.DefinedSettings = current.ToArray();
                Log.Debug($"VVUP CR SSSS: Appended settings. Total now: {current.Count}");
            }
        }
    }
}