using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;
using UnityEngine;
using VVUP.Base;

namespace VVUP.PostNukeEnhancements
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Is radiation enabled after the nuke detonates?")]
        public bool RadiationEnabled { get; set; } = true;
        [Description("How long is the delay before the radiation starts (in seconds)?")]
        public float RadiationDelay { get; set; } = 300;
        [Description("Radiation damage per damage cycle")]
        public float RadiationDamage { get; set; } = 1f;
        [Description("Radiation damage multiplier for SCPs")]
        public float RadiationDamageScpMultiplier { get; set; } = 25f;
        [Description("Time between radiation damage cycles (in seconds)")]
        public float RadiationIntervalTime { get; set; } = 1f;
        [Description("The message shown to players when radiation starts affecting the area")]
        public string RadiationStartMessage { get; set; } = "You feel a sudden wave of nausea and dizziness as radiation begins to affect the area.";
        [Description("How long is the radiation start message shown (in seconds)?")]
        public float RadiationMessageDuration { get; set; } = 5f;
        [Description("If true, hints will be used, otherwise it will be a broadcast")]
        public bool UseHints { get; set; } = true;
        [Description("The message shown to players when they die from radiation poisoning")]
        public string RadiationDeathMessage { get; set; } = "You have succumbed to radiation poisoning.";
        [Description("The effects that are applied to players while they are being irradiated. Note this will apply the effects every interval time.")]
        public List<ApplyEffects> RadiationEffects { get; set; } = new()
        {
            new()
            {
                EffectType = EffectType.Burned,
                Intensity = 1,
                Duration = 0,
            },
            new()
            {
                EffectType = EffectType.Exhausted,
                Intensity = 1,
                Duration = 0,
            },
            new()
            {
                EffectType = EffectType.Concussed,
                Intensity = 1,
                Duration = 0,
            }
        };
        [Description("What roles are immune to radiation damage?")]
        public List<RoleTypeId> RadiationImmuneRoles { get; set; } = new()
        {
            RoleTypeId.Overwatch,
        };
        [Description("Enables surface blackout after the nuke detonates")]
        public bool SurfaceBlackout { get; set; } = true;
        [Description("How long is the surface blackout duration (in seconds)?")]
        public float SurfaceBlackoutDuration { get; set; } = 10f;
        [Description("When the surface blackout ends, what color should the surface lights start at")]
        public Color SurfaceStartColor { get; set; } = new Color(-1, -1, -1, -1);
        [Description("When the surface blackout ends, what color should the surface lights end at. Use -1 for default color")]
        public Color SurfaceEndColor { get; set; } = new Color(-1, -1, -1, -1);
        [Description("Determines if the surface light color be instant or gradual to the target color (this will be after the blackout ends, this will use Surface End Color)")]
        public bool SurfaceColorInstant { get; set; } = false;
        [Description("How often is the surface color updated if not instant (in seconds)")]
        public float SurfaceColorUpdateInterval { get; set; } = 0.1f;
        [Description("How much (about) should the color change each update if not instant (between 0 and 1)")]
        public float SurfaceColorChangeAmount { get; set; } = 0.01f;
        [Description("Should the Surface Gate close after the nuke detonates?")]
        public bool SurfaceGateClose { get; set; } = true;
        [Description("Should the Surface Gate unlock after the nuke detonates?")]
        public bool SurfaceGateUnlock { get; set; } = true;
    }
}