using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;
using UnityEngine;

namespace VVUP.CustomEscapes
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Default Location Escape Handlers")]
        public List<EscapeHandler> DefaultEscapeHandler { get; set; } = new List<EscapeHandler>
        {
            new()
            {
                OriginalRole = RoleTypeId.Scientist,
                NewRole = RoleTypeId.NtfSpecialist,
            },
            new()
            {
                OriginalRole = RoleTypeId.ClassD,
                NewRole = RoleTypeId.None,
            },
            new()
            {
                OriginalRole = RoleTypeId.ClassD,
                NewRole = RoleTypeId.NtfPrivate,
            },
            new()
            {
                OriginalRole = RoleTypeId.ChaosConscript,
                NewRole = RoleTypeId.NtfPrivate,
                Detained = true
            },
            new()
            {
                OriginalRole = RoleTypeId.ChaosRifleman,
                NewRole = RoleTypeId.NtfPrivate,
                Detained = true
            },
            new()
            {
                OriginalRole = RoleTypeId.ChaosMarauder,
                NewRole = RoleTypeId.NtfPrivate,
                Detained = true
            },
            new()
            {
                OriginalRole = RoleTypeId.ChaosRepressor,
                NewRole = RoleTypeId.NtfPrivate,
                Detained = true
            },
            
        };
        [Description("Custom Location Escape Handlers")]
        public List<CustomEscapeHandler> CustomEscapeHandlers { get; set; } = new List<CustomEscapeHandler>
        {
            new()
            {
                Position = new Vector3(-39, 291, -36),
                Handlers = new List<EscapeHandler>
                {
                    new()
                    {
                        OriginalRole = RoleTypeId.ClassD,
                        NewRole = RoleTypeId.ChaosConscript,
                    },
                    new()
                    {
                        OriginalRole = RoleTypeId.Scientist,
                        NewRole = RoleTypeId.ChaosConscript,
                        Detained = true,
                    },
                    new()
                    {
                        OriginalRole = RoleTypeId.FacilityGuard,
                        NewRole = RoleTypeId.ChaosConscript,
                        Detained = true,
                    },
                    new()
                    {
                        OriginalRole = RoleTypeId.NtfSpecialist,
                        NewRole = RoleTypeId.ChaosConscript,
                        Detained = true,
                    },
                    new()
                    {
                        OriginalRole = RoleTypeId.NtfCaptain,
                        NewRole = RoleTypeId.ChaosConscript,
                        Detained = true,
                    },
                    new()
                    {
                        OriginalRole = RoleTypeId.NtfPrivate,
                        NewRole = RoleTypeId.ChaosConscript,
                        Detained = true,
                    },
                    new()
                    {
                        OriginalRole = RoleTypeId.NtfSergeant,
                        NewRole = RoleTypeId.ChaosConscript,
                        Detained = true,
                    },
                }
            },
        };
    }

    public class CustomEscapeHandler
    {
        public Vector3 Position { get; set; }
        public List<EscapeHandler> Handlers { get; set; } = new List<EscapeHandler>();
    }
    
    public class EscapeHandler
    {
        public RoleTypeId OriginalRole { get; set; } = RoleTypeId.None;
        public RoleTypeId NewRole { get; set; } = RoleTypeId.None;
        public bool Detained { get; set; } = false;
        public string EscapeMessage { get; set; }
        public bool UseHints { get; set; }
        public float MessageDuration { get; set; }
    }
}