using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using UserSettings.ServerSpecific;
using VVUP.CustomRoles.Abilities.Active;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.CustomRoles.EventHandlers
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
            if (settingBase is SSKeybindSetting { SyncIsPressed: true } ssKeybindSetting)
            {
                var abilityMap = new Dictionary<int, (Type abilityType, string successMessage)>
                {
                    { Plugin.Instance.Config.SsssConfig.ActiveCamoId, (typeof(ActiveCamo), Plugin.Instance.Config.SsssConfig.SsssActiveCamoActivationMessage) },
                    { Plugin.Instance.Config.SsssConfig.ChargeId, (typeof(ChargeAbility), Plugin.Instance.Config.SsssConfig.SsssChargeActivationMessage) },
                    { Plugin.Instance.Config.SsssConfig.DetectId, (typeof(Detect), null) },
                    { Plugin.Instance.Config.SsssConfig.DoorPickingId, (typeof(DoorPicking), Plugin.Instance.Config.SsssConfig.SsssDoorPickingActivationMessage) },
                    { Plugin.Instance.Config.SsssConfig.HealingMistId, (typeof(HealingMist), Plugin.Instance.Config.SsssConfig.SsssHealingMistActivationMessage) },
                    { Plugin.Instance.Config.SsssConfig.RemoveDisguiseId, (typeof(RemoveDisguise), Plugin.Instance.Config.SsssConfig.SsssRemoveDisguiseActivationMessage) },
                    // { Plugin.Instance.Config.SsssConfig.ReviveMistId, (typeof(RevivingMist), Plugin.Instance.Config.SsssConfig.SsssReviveMistActivationMessage) },
                    { Plugin.Instance.Config.SsssConfig.TeleportId, (typeof(Teleport), Plugin.Instance.Config.SsssConfig.SsssTeleportActivationMessage) },
                    { Plugin.Instance.Config.SsssConfig.SoundBreakerId, (typeof(SoundBreaker), Plugin.Instance.Config.SsssConfig.SsssSoundBreakerActivationMessage) },
                    { Plugin.Instance.Config.SsssConfig.ReplicatorId, (typeof(Replicator), Plugin.Instance.Config.SsssConfig.SsssReplicatorActivationMessage) }
                };

                if (abilityMap.TryGetValue(ssKeybindSetting.SettingId, out var abilityInfo) && 
                    ActiveAbility.AllActiveAbilities.TryGetValue(player, out var abilities))
                {
                    string response = string.Empty;
                    var ability = abilities.FirstOrDefault(a => a.GetType() == abilityInfo.abilityType);
        
                    if (ability != null && ability.CanUseAbility(player, out response))
                    {
                        ability.SelectAbility(player);
                        ability.UseAbility(player);
            
                        if (abilityInfo.abilityType == typeof(HealingMist))
                            if (Plugin.Instance.Config.SsssConfig.UseHints)
                                player.ShowHint(response);
                            else
                                player.Broadcast(3, response);
                
                        if (abilityInfo.successMessage != null)
                            if (Plugin.Instance.Config.SsssConfig.UseHints)
                                player.ShowHint(abilityInfo.successMessage);
                            else
                                player.Broadcast(3, abilityInfo.successMessage);
                    }
                    else
                    {
                        if (Plugin.Instance.Config.SsssConfig.UseHints)
                            player.ShowHint(response);
                        else
                            player.Broadcast(3, response);
                    }
                }
            }
        }
    }
}