using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using NorthwoodLib.Pools;
using UserSettings.ServerSpecific;
using VVUP.FreeCustomRoles.FreeCustomRoles;

namespace VVUP.FreeCustomRoles
{
    public class SsssHelper
    {
        public static ServerSpecificSettingBase[] GetSettings()
        {
            List<ServerSpecificSettingBase> settings = new List<ServerSpecificSettingBase>();
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            var customRoles = new List<CustomRole>
            {
                FreeCustomRole1.Get(typeof(FreeCustomRole1)),
                FreeCustomRole2.Get(typeof(FreeCustomRole2)),
                FreeCustomRole3.Get(typeof(FreeCustomRole3)),
                FreeCustomRole4.Get(typeof(FreeCustomRole4)),
                FreeCustomRole5.Get(typeof(FreeCustomRole5)),
                FreeCustomRole6.Get(typeof(FreeCustomRole6)),
                FreeCustomRole7.Get(typeof(FreeCustomRole7)),
                FreeCustomRole8.Get(typeof(FreeCustomRole8)),
                FreeCustomRole9.Get(typeof(FreeCustomRole9)),
                FreeCustomRole10.Get(typeof(FreeCustomRole10)),
                FreeCustomRole11.Get(typeof(FreeCustomRole11)),
                FreeCustomRole12.Get(typeof(FreeCustomRole12)),
                FreeCustomRole13.Get(typeof(FreeCustomRole13)),
                FreeCustomRole14.Get(typeof(FreeCustomRole14)),
                FreeCustomRole15.Get(typeof(FreeCustomRole15)),
                FreeCustomRole16.Get(typeof(FreeCustomRole16)),
                FreeCustomRole17.Get(typeof(FreeCustomRole17)),
                FreeCustomRole18.Get(typeof(FreeCustomRole18)),
                FreeCustomRole19.Get(typeof(FreeCustomRole19)),
                FreeCustomRole20.Get(typeof(FreeCustomRole20)),
            };
                
            foreach (var role in customRoles.Where(role => role?.CustomAbilities != null))
            {
                stringBuilder.AppendLine($"Role: {role.Name}");
                stringBuilder.AppendLine($"- Description: {role.Description}");
                foreach (var ability in role.CustomAbilities)
                {
                    stringBuilder.AppendLine($"-- Ability: {ability.Name}, {ability.Description}");
                }
            }
            settings.Add(new SSTextArea(Plugin.Instance.Config.FreeCustomRoleTextId, StringBuilderPool.Shared.ToStringReturn(stringBuilder),
                SSTextArea.FoldoutMode.CollapsedByDefault));
            stringBuilder.Clear();
            return settings.ToArray();
        }
        public static void SafeAppendSsssSettings()
        {
            var mySettings = GetSettings();
            var current = ServerSpecificSettingsSync.DefinedSettings?.ToList() ?? new List<ServerSpecificSettingBase>();
            bool needToAddSettings = mySettings.Any(setting => current.All(s => s.SettingId != setting.SettingId));
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
                        Log.Debug($"VVUP FCR SSSS: Skipped duplicate SettingId: {setting.SettingId}");
                }
        
                ServerSpecificSettingsSync.DefinedSettings = current.ToArray();
                Log.Debug($"VVUP FCR SSSS: Appended settings. Total now: {current.Count}");
            }
        }
    }
}