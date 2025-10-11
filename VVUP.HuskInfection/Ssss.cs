using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using NorthwoodLib.Pools;
using UserSettings.ServerSpecific;

namespace VVUP.HuskInfection
{
    public class SsssHelper
    {
        public static ServerSpecificSettingBase[] GetSettings()
        {
            List<ServerSpecificSettingBase> settings = new List<ServerSpecificSettingBase>();
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            var customRoles = new List<CustomRole>
            {
                HuskZombie.Get(typeof(HuskZombie)),
            };
                
            foreach (var role in customRoles.Where(role => role is { CustomAbilities: not null }))
            {
                stringBuilder.AppendLine($"Role: {role.Name}");
                stringBuilder.AppendLine($"- Description: {role.Description}");
                foreach (var ability in role.CustomAbilities)
                {
                    stringBuilder.AppendLine($"-- Ability: {ability.Name}, {ability.Description}");
                }
            }
            var customItems = new List<IEnumerable<CustomItem>>
            {
                HuskGrenade.Get(typeof(HuskGrenade)),
                Calyxanide.Get(typeof(Calyxanide)),
            };

            foreach (var items in customItems.Where(itemCollection => itemCollection != null).SelectMany(itemCollection => itemCollection))
            {
                stringBuilder.AppendLine($"Item: {items.Name}");
                stringBuilder.AppendLine($"- Description: {items.Description}");
            }
            settings.Add(new SSTextArea(Plugin.Instance.Config.HuskInfectionTextId, StringBuilderPool.Shared.ToStringReturn(stringBuilder),
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
                        Log.Debug($"VVUP HK SSSS: Skipped duplicate SettingId: {setting.SettingId}");
                }
        
                ServerSpecificSettingsSync.DefinedSettings = current.ToArray();
                Log.Debug($"VVUP HK SSSS: Appended settings. Total now: {current.Count}");
            }
        }
    }
}