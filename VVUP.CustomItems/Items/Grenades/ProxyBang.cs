using System.ComponentModel;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;
using PlayerRoles;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class ProxyBang : CustomGrenade, ICustomItemGlow
    {
        [YamlIgnore] public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override uint Id { get; set; } = 43;
        public override string Name { get; set; } = "<color=#6600CC>Pathfinder Grenade</color>";

        public override string Description { get; set; } =
            "When detonates, it shows lines to all players in the area.";

        public override float Weight { get; set; } = 1.75f;
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 15,
                    Location = SpawnLocationType.InsideHidChamber,
                },
            },
        };

        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 10;
        public float Range { get; set; } = 100;
        public float LineVisibleTime { get; set; } = 5;

        public List<RoleColor> RoleColorBeam = new()
        {
            new RoleColor()
            {
                Role = RoleTypeId.NtfCaptain,
                R = 0,
                G = 0.23921568627450981f,
                B = 0.792156862f,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.NtfSergeant,
                R = 0,
                G = 0.5882352941176471f,
                B = 1,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.NtfSpecialist,
                R = 0,
                G = 0.8509803921568627f,
                B = 1,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.NtfPrivate,
                R = 0.4392156862745098f,
                G = 0.7647058823529411f,
                B = 1,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.FacilityGuard,
                R = 0.3568627450980392f,
                G = 0.38823529411764707f,
                B = 0.4392156862745098f,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.Scientist,
                R = 1,
                G = 1,
                B = 0.48627450980392156f,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.ClassD,
                R = 1,
                G = 0.5568627450980392f,
                B = 0,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.ChaosRifleman,
                R = 0,
                G = 0.5607843137254902f,
                B = 0.10980392156862745f,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.ChaosRepressor,
                R = 0.08235294117647059f,
                G = 0.5215686274509804f,
                B = 0.23921568627450981f,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.ChaosMarauder,
                R = 0.023529411764705882f,
                G = 0.38823529411764707f,
                B = 0.1568627450980392f,
                A = 1,
            },
            new RoleColor()
            {
                Role = RoleTypeId.ChaosConscript,
                R = 0.3333333333333333f,
                G = 0.5686274509803921f,
                B = 0.00392156862745098f,
                A = 1,
            },
            new RoleColor()
            {
                IsCustomRole = true,
                CustomRoleId = 25,
                R = 0.9686274509803922f,
                G = 0,
                B = 0.9921568627450981f,
                A = 1,
            },
            new RoleColor()
            {
                IsCustomRole = true,
                CustomRoleId = 26,
                R = 0.9686274509803922f,
                G = 0,
                B = 0.9921568627450981f,
                A = 1,
            },
            new RoleColor()
            {
                IsCustomRole = true,
                CustomRoleId = 27,
                R = 0.9686274509803922f,
                G = 0,
                B = 0.9921568627450981f,
                A = 1,
            },
            new RoleColor()
            {
                IsCustomRole = true,
                CustomRoleId = 20,
                R = 1,
                G = 1,
                B = 0,
                A = 1,
            },
            new RoleColor()
            {
                IsCustomRole = true,
                CustomRoleId = 21,
                R = 1,
                G = 1,
                B = 0,
                A = 1,
            },
            new RoleColor()
            {
                IsCustomRole = true,
                CustomRoleId = 22,
                R = 1,
                G = 1,
                B = 0,
                A = 1,
            },
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            foreach (PlayerAPI player in PlayerAPI.List)
            {
                if (Vector3.Distance(ev.Position, player.Position) <= Range)
                {
                    var color = GetTeamColor(player);
                    var lineColor = new Color(color.red, color.green, color.blue, color.alpha);
                    var direction = player.Position - ev.Position;
                    var distance = direction.magnitude;
                    var scale = new Vector3(0.1f, distance * 0.5f, 0.1f);
                    var laserPos = ev.Position + direction * 0.5f;
                    var rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
                    Log.Debug($"VVUP Custom Items: Proxy Bang, Laser Info: Position: {laserPos}, Rotation: {rotation.eulerAngles}, Color: {lineColor}");
                    var laser = Primitive.Create(PrimitiveType.Cylinder, PrimitiveFlags.Visible, laserPos, rotation.eulerAngles,
                        scale, true, lineColor);
                    Timing.CallDelayed(LineVisibleTime, laser.Destroy);
                }
            }
        }

        private (float red, float green, float blue, float alpha) GetTeamColor(PlayerAPI player)
        {
            if (CustomRole.TryGet(player, out IReadOnlyCollection<CustomRole> customRoles))
            {
                foreach (RoleColor roleColor in RoleColorBeam)
                {
                    if (!roleColor.IsCustomRole)
                        continue;

                    foreach (CustomRole customRole in customRoles)
                    {
                        if (customRole.Id != roleColor.CustomRoleId)
                            continue;

                        return (roleColor.R, roleColor.G, roleColor.B, roleColor.A);
                    }
                }
            }

            foreach (RoleColor roleColor in RoleColorBeam)
            {
                if (roleColor.IsCustomRole || roleColor.Role != player.Role.Type)
                    continue;

                return (roleColor.R, roleColor.G, roleColor.B, roleColor.A);
            }

            return (1f, 1f, 1f, 1f);
        }
        public class RoleColor
        {
            public bool IsCustomRole { get; set; }

            [Description("If IsCustomRole is false, it will use a base game role and CustomRoleId will be ignored")]
            public RoleTypeId Role { get; set; } = RoleTypeId.None;
            [Description("If IsCustomRole is true, it will use a custom role id and base game roles will be ignored")]
            public uint CustomRoleId { get; set; }
            [Description("RGBA is 0-1")]
            public float R { get; set; } = 1;
            public float G { get; set; } = 1;
            public float B { get; set; } = 1;
            public float A { get; set; } = 1;
            public Color GetColor() => new Color(
                Mathf.Clamp(R, 0f, 1f), 
                Mathf.Clamp(G, 0f, 1f), 
                Mathf.Clamp(B, 0f, 1f), 
                Mathf.Clamp(A, 0f, 1f));
        }
    }
}