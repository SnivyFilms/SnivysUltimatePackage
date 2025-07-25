﻿using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Loader;
using SnivysUltimatePackage.API;

namespace SnivysUltimatePackage.EventHandlers.Custom
{
    public class CustomRoleMethods
    {
        private readonly Plugin Plugin;
        public CustomRoleMethods(Plugin plugin) => Plugin = plugin;

        public static CustomRole? GetCustomRole(ref List<ICustomRole>.Enumerator enumerator, bool checkEscape = false,
            bool checkRevive = false)
        {
            try
            {
                Log.Debug("VVUP Custom Roles: Getting role from enumerator..");

                while (enumerator.MoveNext())
                {
                    Log.Debug(enumerator.Current?.StartTeam);
                    if (enumerator.Current is not null)
                    {
                        int r = Loader.Random.Next(100);
                        if (enumerator.Current.StartTeam.HasFlag(StartTeam.Other)
                            || (enumerator.Current.StartTeam.HasFlag(StartTeam.Revived) && !checkRevive)
                            || (enumerator.Current.StartTeam.HasFlag(StartTeam.Escape) && !checkEscape)
                            || (!enumerator.Current.StartTeam.HasFlag(StartTeam.Revived) && checkRevive)
                            || (!enumerator.Current.StartTeam.HasFlag(StartTeam.Escape) && checkEscape)
                            || r > enumerator.Current.Chance)
                        {
                            Log.Debug(
                                $"VVUP Custom Roles: Validation check failed | {enumerator.Current.StartTeam} {enumerator.Current.Chance}% || {r}");
                            continue;
                        }

                        Log.Debug("VVUP Custom Roles: Returning a role!");
                        return (CustomRole)enumerator.Current;
                    }
                }

                Log.Debug("VVUP Custom Roles: Cannot move next");

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}