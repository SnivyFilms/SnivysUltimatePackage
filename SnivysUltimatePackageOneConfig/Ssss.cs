﻿using System.Collections.Generic;
using System.Text;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using NorthwoodLib.Pools;
using SnivysUltimatePackageOneConfig.Custom.Items.Armor;
using SnivysUltimatePackageOneConfig.Custom.Items.Firearms;
using SnivysUltimatePackageOneConfig.Custom.Items.Grenades;
using SnivysUltimatePackageOneConfig.Custom.Items.MedicalItems;
using SnivysUltimatePackageOneConfig.Custom.Items.Other;
using SnivysUltimatePackageOneConfig.Custom.Roles.Chaos;
using SnivysUltimatePackageOneConfig.Custom.Roles.ClassD;
using SnivysUltimatePackageOneConfig.Custom.Roles.Foundation;
using SnivysUltimatePackageOneConfig.Custom.Roles.OpenCustomRoles;
using SnivysUltimatePackageOneConfig.Custom.Roles.Other;
using SnivysUltimatePackageOneConfig.Custom.Roles.Scientist;
using SnivysUltimatePackageOneConfig.Custom.Roles.Scps;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace SnivysUltimatePackageOneConfig
{
    public class SsssHelper
    {
        public static ServerSpecificSettingBase[] GetSettings()
        {
            List<ServerSpecificSettingBase> settings = new List<ServerSpecificSettingBase>();
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            if (Plugin.Instance.Config.CustomRolesConfig.IsEnabled)
            {
                var customRoles = new List<CustomRole>
                {
                    A7Chaos.Get(typeof(A7Chaos)),
                    Biochemist.Get(typeof(Biochemist)),
                    BorderPatrol.Get(typeof(BorderPatrol)),
                    CiPhantom.Get(typeof(CiPhantom)),
                    CISpy.Get(typeof(CISpy)),
                    ContainmentGuard.Get(typeof(ContainmentGuard)),
                    ContainmentScientist.Get(typeof(ContainmentScientist)),
                    Demolitionist.Get(typeof(Demolitionist)),
                    DwarfZombie.Get(typeof(DwarfZombie)),
                    ExplosiveZombie.Get(typeof(ExplosiveZombie)),
                    Flipped.Get(typeof(Flipped)),
                    JuggernautChaos.Get(typeof(JuggernautChaos)),
                    LockpickingClassD.Get(typeof(LockpickingClassD)),
                    MedicZombie.Get(typeof(MedicZombie)),
                    MtfWisp.Get(typeof(MtfWisp)),
                    Nightfall.Get(typeof(Nightfall)),
                    TelepathicChaos.Get(typeof(TelepathicChaos)),
                };

                if (Plugin.Instance.Config.CustomRolesConfig.EnableFreeCustomRoles)
                {
                    var freeCustomRoles = new List<CustomRole>
                    {
                        FreeCustomRole1.Get(typeof(FreeCustomRole1)),
                        FreeCustomRole2.Get(typeof(FreeCustomRole2)),
                        FreeCustomRole3.Get(typeof(FreeCustomRole3)),
                        FreeCustomRole4.Get(typeof(FreeCustomRole4)),
                        FreeCustomRole5.Get(typeof(FreeCustomRole5)),
                    };
                    foreach (var role in freeCustomRoles)
                    {
                        if (role == null || role.CustomAbilities == null) continue;

                        stringBuilder.AppendLine($"Role: {role.Name}");
                        stringBuilder.AppendLine($"- Description: {role.Description}");
                        foreach (var ability in role.CustomAbilities)
                        {
                            stringBuilder.AppendLine($"-- Ability: {ability.Name}, {ability.Description}");
                        }
                    }
                }

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
                
                settings.Add(new SSGroupHeader("VV Custom Roles Info"));
                settings.Add(new SSTextArea(null, StringBuilderPool.Shared.ToStringReturn(stringBuilder),
                    SSTextArea.FoldoutMode.CollapsedByDefault));
                stringBuilder.Clear();
            }

            if (Plugin.Instance.Config.CustomItemsConfig.IsEnabled)
            {
                var customItems = new List<IEnumerable<CustomItem>>
                {
                    ExplosiveResistantArmor.Get(typeof(ExplosiveResistantArmor)),
                    ExplosiveRoundRevolver.Get(typeof(ExplosiveRoundRevolver)),
                    //MediGun.Get(typeof(MediGun)),
                    Tranquilizer.Get(typeof(Tranquilizer)),
                    C4.Get(typeof(C4)),
                    EmpGrenade.Get(typeof(EmpGrenade)),
                    NerveAgentGrenade.Get(typeof(NerveAgentGrenade)),
                    SmokeGrenade.Get(typeof(SmokeGrenade)),
                    DeadringerSyringe.Get(typeof(DeadringerSyringe)),
                    KySyringe.Get(typeof(KySyringe)),
                    AntiScp096Pills.Get(typeof(AntiScp096Pills)),
                    InfinitePills.Get(typeof(InfinitePills)),
                    PhantomLantern.Get(typeof(PhantomLantern)),
                    Scp1499.Get(typeof(Scp1499)),
                    ClusterGrenade.Get(typeof(ClusterGrenade)),
                    AdditionalHealth207.Get(typeof(AdditionalHealth207)),
                    LowGravityArmor.Get(typeof(LowGravityArmor)),
                    ViperPdw.Get(typeof(ViperPdw)),
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

                settings.Add(new SSGroupHeader("VV Custom Items Info"));
                settings.Add(new SSTextArea(null, StringBuilderPool.Shared.ToStringReturn(stringBuilder),
                    SSTextArea.FoldoutMode.CollapsedByDefault));
                stringBuilder.Clear();
            }

            if (Plugin.Instance.Config.CustomRolesConfig.IsEnabled &&
                Plugin.Instance.Config.CustomRolesAbilitiesConfig.IsEnabled)
            {
                settings.Add(new SSGroupHeader("VV Custom Abilities Key Activators"));
                settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.ActiveCamoId, "Active Camo",
                    KeyCode.B, true, "B"));
                settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.ChargeId, "Charge", KeyCode.B, true,
                    "B"));
                settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.DetectId, "Detect", KeyCode.B, true,
                    "B"));
                settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.DoorPickingId, "Door Picking",
                    KeyCode.B, true, "B"));
                settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.HealingMistId, "Healing Mist",
                    KeyCode.B, true, "B"));
                settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.RemoveDisguiseId, "Remove Disguise",
                    KeyCode.B, true, "B"));
            }

            if (Plugin.Instance.Config.CustomItemsConfig.IsEnabled)
            {
                settings.Add(new SSGroupHeader("VV Custom Items Activators"));
                settings.Add(new SSKeybindSetting(Plugin.Instance.Config.SsssConfig.DetonateC4Id, "Detonate C4",
                    KeyCode.J, true, "J"));
            }

            return settings.ToArray();
        }
    }
}