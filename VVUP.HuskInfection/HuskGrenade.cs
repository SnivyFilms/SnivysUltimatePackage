using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using PlayerRoles;
using UnityEngine;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.HuskInfection
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class HuskGrenade : CustomGrenade
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override uint Id { get; set; } = 48;
        public override string Name { get; set; } = "<color=#FF0000>Husk Grenade</color>";
        public override string Description { get; set; } = "A throwable grenade that contains a Husk Infection Virus. When it explodes, it infects all nearby players with the Husk Infection.";
        public override float Weight { get; set; } = 0.75f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside096,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 4;
        public float Range { get; set; } = 5f;
        public bool IgnoreTutorials { get; set; } = false;
        [Description("How long it takes for the infection to reach stage 1, in seconds")]
        public float InfectionStageOneDelay { get; set; } = 30f;
        [Description("How long it takes for the infection to reach stage 2, in seconds **AFTER** stage 1 is reached")]
        public float InfectionStageTwoDelay { get; set; } = 90f;
        [Description("The text that will be shown once stage 2 is reached")]
        public string InfectionText { get; set; } = "<color=red><size=30>You feel something in your throat. You try to scream, but nothing comes out.</size></color>";
        public bool UseHints { get; set; } = false;
        public float TextDisplayTime { get; set; } = 10f;
        public uint HuskZombieCustomRoleId { get; set; } = 56;
        public string HuskTakeOverDeathReason { get; set; } = "You have been taken over by a Husk Infection.";

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            foreach (PlayerAPI player in PlayerAPI.List)
            {
                if (Vector3.Distance(ev.Position, player.Position) <= Range)
                {
                    if (player.IsScp)
                        continue;
                    if (IgnoreTutorials && player.Role.Type == RoleTypeId.Tutorial)
                        continue;
                    if (HuskInfectionEventHandlers.PlayersWithHuskInfection.ContainsKey(player))
                        continue;
                    Log.Debug($"VVUP Custom Items: HuskGrenade, Husk Grenade infecting {player.Nickname}.");
                    HuskInfectionEventHandlers huskInfection = new HuskInfectionEventHandlers(player, InfectionStageOneDelay, InfectionStageTwoDelay, 
                        InfectionText, UseHints, TextDisplayTime, HuskZombieCustomRoleId, HuskTakeOverDeathReason);
                }
            }
        }
    }
}