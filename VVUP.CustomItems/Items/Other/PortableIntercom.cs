using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using MEC;
using PlayerRoles;
using PlayerRoles.Voice;
using UnityEngine;
using YamlDotNet.Serialization;
using Intercom = Exiled.API.Features.Intercom;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.Radio)]
    public class PortableIntercom : CustomItem
    {
        [YamlIgnore] 
        public override ItemType Type { get; set; } = ItemType.Radio;
        public override uint Id { get; set; } = 47;
        public override string Name { get; set; } = "Portable Intercom";

        public override string Description { get; set; } =
            "A portable intercom that can be used to communicate to the entire facility.";

        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
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
                    Location = SpawnLocationType.Inside096,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideGr18,
                },
            },
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Role = RoleTypeId.Scp106
                }
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Room = RoomType.HczTestRoom,
                    Offset = new Vector3(0.885f, 0.749f, -4.874f)
                }
            }
        };
        public float PortableIntercomDuration { get; set; } = 20f;
        [Description("Use %intercomtimeremaining% to show how much time is remaining on the portable intercom.")]
        public string CountdownTextForPortableIntercom { get; set; } =
            "Your portable intercom will end in %intercomtimeremaining% seconds.";
        [Description("If true, if the portable intercom is active, toggling it again will end the intercom.")]
        public bool EndIntercomOnSecondInteraction { get; set; } = true;
        public bool UseHints { get; set; } = true;
        [Description("If true, the portable intercom will respect stuff like the intercom cooldown and the intercom being disabled.")]
        public bool FollowActualIntercomParameters { get; set; } = true;
        public string PortableIntercomFailDueToIntercomCooldownText { get; set; } =
            "The intercom is currently on cooldown. Please wait before using it again.";

        public string IntercomRoomPortableIntercomInUseText { get; set; } =
            "The intercom is currently in use in a remote location.";
        private bool isPortableIntercomActive = false;
        private static CoroutineHandle _portableIntercomCoroutine;
        private List<Player> _playerWithPortableIntercom = new List<Player>();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.TogglingRadio -= OnTogglingRadio;
            base.UnsubscribeEvents();
        }

        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            if (_playerWithPortableIntercom.Contains(ev.Player) && isPortableIntercomActive)
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is changing item while using the portable intercom, removing them from the list.");
                _playerWithPortableIntercom.Remove(ev.Player);
            }
            base.OnChanging(ev);
        }
        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (_playerWithPortableIntercom.Contains(ev.Player) && isPortableIntercomActive)
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is dropping item while using the portable intercom, removing them from the list.");
                _playerWithPortableIntercom.Remove(ev.Player);
            }
            base.OnDroppingItem(ev);
        }

        protected override void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev)
        {
            if (_playerWithPortableIntercom.Contains(ev.Player) && isPortableIntercomActive &&
                Check(ev.Player.CurrentItem))
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is changing role while using the portable intercom, removing them from the list.");
                _playerWithPortableIntercom.Remove(ev.Player);
            }
            base.OnOwnerChangingRole(ev);
        }

        protected override void OnWaitingForPlayers()
        {
            Log.Debug("VVUP Custom Items, Portable Intercom: At Waiting for players, clearing portable intercom data.");
            _playerWithPortableIntercom.Clear();
            isPortableIntercomActive = false;
            if (_portableIntercomCoroutine.IsRunning)
                Timing.KillCoroutines(_portableIntercomCoroutine);
            base.OnWaitingForPlayers();
        }

        private void OnTogglingRadio(TogglingRadioEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            if (isPortableIntercomActive && EndIntercomOnSecondInteraction && _playerWithPortableIntercom.Contains(ev.Player))
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is toggling the portable intercom while it is active, ending the intercom.");
                _playerWithPortableIntercom.Remove(ev.Player);
            }
            else if (isPortableIntercomActive && _playerWithPortableIntercom.Contains(ev.Player))
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is toggling the portable intercom while it is active, but EndIntercomOnSecondInteraction is false, ignoring.");
                return;
            }

            if (FollowActualIntercomParameters && Intercom.State != IntercomState.Ready)
            {
                if (UseHints)
                    ev.Player.ShowHint(PortableIntercomFailDueToIntercomCooldownText, 5);
                else
                    ev.Player.Broadcast(5, PortableIntercomFailDueToIntercomCooldownText);
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} tried to use the portable intercom, but it is on cooldown.");
                return;
            }
            Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is toggling the portable intercom, starting the intercom, adding to list, setting Intercom State, setting Intercom Display, setting {ev.Player.Nickname} voice channel, Starting Coroutine");
            isPortableIntercomActive = true;
            ev.IsAllowed = false;
            ev.Radio.IsEnabled = true;
            _playerWithPortableIntercom.Add(ev.Player);
            Intercom.PlaySound(true);
            Intercom.State = IntercomState.Cooldown;
            Intercom.DisplayText = IntercomRoomPortableIntercomInUseText;
            Timing.CallDelayed(1.5f, () => Server.ExecuteCommand($"/icom {ev.Player.Id} 1"));
            _portableIntercomCoroutine = Timing.RunCoroutine(PortableIntercomTiming(ev.Player));
        }
        private IEnumerator<float> PortableIntercomTiming(Player player)
        {
            float time = PortableIntercomDuration;
            while (time > 0)
            {
                if (!_playerWithPortableIntercom.Contains(player))
                {
                    Log.Debug($"VVUP Custom Items, Portable Intercom: {player.Nickname} is no longer in the list of players with the portable intercom, ending the intercom.");
                    isPortableIntercomActive = false;
                    Intercom.PlaySound(false);
                    Intercom.DisplayText = string.Empty;
                    Intercom.State = IntercomState.Cooldown;
                    Server.ExecuteCommand($"/icom {player.Id} 0");
                    yield break;
                }
                yield return Timing.WaitForSeconds(1f);
                time--;
                Log.Debug($"VVUP Custom Items, Portable Intercom: {player.Nickname} portable intercom time remaining: {time}, showing message to {player.Nickname} showing how much time they have remaining. Waiting 1 Second.");
                string message = ProcessStringVaribles(CountdownTextForPortableIntercom, time);
                if (UseHints)
                    player.ShowHint(message, 1);
                else
                    player.Broadcast(1, message, shouldClearPrevious:true);
            }
            Log.Debug($"VVUP Custom Items, Portable Intercom: {player.Nickname} portable intercom has ended, removing from list and ending the intercom.");
            isPortableIntercomActive = false;
            Intercom.PlaySound(false);
            _playerWithPortableIntercom.Remove(player);
            Server.ExecuteCommand($"/icom {player.Id} 0");
            yield break;
        }
        private string ProcessStringVaribles(string text, float time)
        {
            return text.Replace("%intercomtimeremaining%", Math.Floor(time).ToString());
        }
    }
}