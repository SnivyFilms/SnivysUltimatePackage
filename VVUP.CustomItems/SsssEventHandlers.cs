﻿using System.Linq;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using UserSettings.ServerSpecific;
using VVUP.CustomItems.Items.Firearms;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.CustomItems
{
    public class SsssEventHandlers
    {
        public Plugin Plugin;
        public SsssEventHandlers(Plugin plugin) => Plugin = plugin;

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (Plugin.Instance.SsssEventHandlers == null)
                return;
            if (!Plugin.Instance.Config.SsssConfig.SsssEnabled)
                return;
            
            Log.Debug($"VVUP: Adding SSSS functions to {ev.Player.Nickname}");
            SsssHelper.SafeAppendSsssSettings();
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }

        public static void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!PlayerAPI.TryGet(hub, out PlayerAPI player) || hub == null || player == null)
                return;
            if (settingBase is not SSKeybindSetting { SyncIsPressed: true } ssKeybindSetting)
                return;

            if (ssKeybindSetting.SettingId == Plugin.Instance.Config.SsssConfig.DetonateC4Id)
            {
                bool hasC4Charges = Items.Grenades.C4.PlacedCharges.ContainsValue(player);
                bool hasF4Charges = Items.Grenades.F4.PlacedCharges.ContainsValue(player);

                if (!hasC4Charges && !hasF4Charges)
                {
                    player.ShowHint(Plugin.Instance.Config.SsssConfig.SsssC4NoC4Deployed);
                    return;
                }

                bool needsC4Detonator = Items.Grenades.C4.Instance.RequireDetonator && hasC4Charges;
                bool needsF4Detonator = Items.Grenades.F4.Instance.RequireDetonator && hasF4Charges;

                if ((needsC4Detonator || needsF4Detonator) &&
                    (player.CurrentItem is null ||
                     (needsC4Detonator && player.CurrentItem.Type != Items.Grenades.C4.Instance.DetonatorItem) ||
                     (needsF4Detonator && player.CurrentItem.Type != Items.Grenades.F4.Instance.DetonatorItem)))
                {
                    player.ShowHint(Plugin.Instance.Config.SsssConfig.SsssC4DetonatorNeeded);
                    return;
                }
                
                bool anyChargeDetonated = false;

                // C4
                foreach (var charge in Items.Grenades.C4.PlacedCharges.ToList())
                {
                    if (charge.Value != player)
                        continue;

                    float distance = Vector3.Distance(charge.Key.Position, player.Position);
                    if (distance < Items.Grenades.C4.Instance.MaxDistance)
                    {
                        Items.Grenades.C4.Instance.C4Handler(charge.Key);
                        anyChargeDetonated = true;
                    }
                    else
                        player.ShowHint(Plugin.Instance.Config.SsssConfig.SsssC4TooFarAway);
                }

                // F4
                foreach (var charge in Items.Grenades.F4.PlacedCharges.ToList())
                {
                    if (charge.Value != player)
                        continue;

                    float distance = Vector3.Distance(charge.Key.Position, player.Position);
                    if (distance < Items.Grenades.F4.Instance.MaxDistance)
                    {
                        Items.Grenades.F4.Instance.F4Handler(charge.Key);
                        anyChargeDetonated = true;
                    }
                    else 
                        player.ShowHint(Plugin.Instance.Config.SsssConfig.SsssC4TooFarAway);
                }

                if (anyChargeDetonated)
                    player.ShowHint(Plugin.Instance.Config.SsssConfig.SsssDetonateC4ActivationMessage);
            }
            else if (ssKeybindSetting.SettingId == Plugin.Instance.Config.SsssConfig.GrenadeLauncherForceModeId ||
                     ssKeybindSetting.SettingId == Plugin.Instance.Config.SsssConfig.GrenadeLauncherLaunchModeId)
            {
                if (!CustomItem.TryGet(player.CurrentItem, out var customItem) ||
                    customItem is not GrenadeLauncher grenadeLauncher)
                    return;

                if (ssKeybindSetting.SettingId == Plugin.Instance.Config.SsssConfig.GrenadeLauncherForceModeId)
                    grenadeLauncher.ToggleForceMode(player);
                else
                    grenadeLauncher.ToggleLaunchTypeMode(player);
            }
        }
    }
}