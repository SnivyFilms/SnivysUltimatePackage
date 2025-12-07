using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.CustomEscapes
{
    public class EventHandlers
    {
        public Plugin Plugin;
        public EventHandlers(Plugin plugin) => Plugin = plugin;

        public void OnRoundStarted()
        {
            foreach (var customEscapeLocation in Plugin.Config.CustomEscapeHandlers)
            {
                GameObject customEscapePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                customEscapePrimitive.isStatic = true;
                customEscapePrimitive.transform.position = customEscapeLocation.Position;
                customEscapePrimitive.GetComponent<BoxCollider>().isTrigger = true;
                customEscapePrimitive.AddComponent<EscapeHandle>().Init(customEscapeLocation.Handlers);
                Log.Debug($"VVUP Custom Escapes: Spawned Custom Escape Handler at {customEscapeLocation.Position}");
            }
        }
        
        public void OnDefaultEscape(EscapingEventArgs ev)
        {
            foreach (EscapeHandler defaultEscapeHandler in Plugin.Instance.Config.DefaultEscapeHandler)
            {
                if (defaultEscapeHandler.OriginalRole != ev.Player.Role.Type)
                {
                    Log.Debug($"VVUP Custom Escapes: Player {ev.Player.Nickname} role {ev.Player.Role.Type} does not match required role {defaultEscapeHandler.OriginalRole}");
                    continue;
                }

                if (defaultEscapeHandler.Detained != ev.Player.IsCuffed)
                {
                    Log.Debug($"VVUP Custom Escapes: Player {ev.Player.Nickname} cuffed state {ev.Player.IsCuffed} does not match required cuffed state {defaultEscapeHandler.Detained}");
                    continue;
                }
                
                ev.EscapeScenario = EscapeScenario.CustomEscape;
                ev.NewRole = defaultEscapeHandler.NewRole;
                ev.IsAllowed = ev.NewRole != RoleTypeId.None;
                
                if (ev.IsAllowed && !string.IsNullOrWhiteSpace(defaultEscapeHandler.EscapeMessage))
                {
                    Log.Debug($"VVUP Custom Escapes: Showing escape message to {ev.Player.Nickname}");
                    if (defaultEscapeHandler.UseHints)
                        ev.Player.ShowHint(defaultEscapeHandler.EscapeMessage, defaultEscapeHandler.MessageDuration);
                    else
                        ev.Player.Broadcast((ushort)defaultEscapeHandler.MessageDuration, defaultEscapeHandler.EscapeMessage);
                }

                return;
            }
        }
    }

    public class EscapeHandle : MonoBehaviour
    {
        private List<EscapeHandler> customEscapeHandler = null!;

        public void Init(List<EscapeHandler> handles)
        {
            customEscapeHandler = handles;
        }

       private void OnTriggerEnter(Collider collider)
        {
            if (!PlayerAPI.TryGet(collider, out PlayerAPI player))
                return;

            foreach (EscapeHandler customEscapeHandlers in customEscapeHandler)
            {
                if (customEscapeHandlers.OriginalRole != player.Role.Type)
                {
                    Log.Debug($"VVUP Custom Escapes: Player {player.Nickname} role {player.Role.Type} does not match required role {customEscapeHandlers.OriginalRole}");
                    continue;
                }

                if (customEscapeHandlers.Detained != player.IsCuffed)
                {
                    Log.Debug($"VVUP Custom Escapes: Player {player.Nickname} cuffed state {player.IsCuffed} does not match required cuffed state {customEscapeHandlers.Detained}");
                    continue;
                }

                var escapingEvent = new EscapingEventArgs(player.ReferenceHub, customEscapeHandlers.NewRole, EscapeScenario.CustomEscape);
                Exiled.Events.Handlers.Player.OnEscaping(escapingEvent);

                if (player.Role.Type != customEscapeHandlers.NewRole)
                {
                    Log.Debug($"VVUP Custom Escapes: Changing {player.Nickname} role from {player.Role.Type} to {customEscapeHandlers.NewRole} due to escape. Player is NOT a custom role");
                    player.Role.Set(customEscapeHandlers.NewRole, SpawnReason.Escaped);
                    if (!string.IsNullOrWhiteSpace(customEscapeHandlers.EscapeMessage))
                    {
                        Log.Debug($"VVUP Custom Escapes: Showing escape message to {player.Nickname}");
                        if (customEscapeHandlers.UseHints)
                            player.ShowHint(customEscapeHandlers.EscapeMessage, customEscapeHandlers.MessageDuration);
                        else
                            player.Broadcast((ushort)customEscapeHandlers.MessageDuration, customEscapeHandlers.EscapeMessage);
                    }
                }
            }
        }
    }
}