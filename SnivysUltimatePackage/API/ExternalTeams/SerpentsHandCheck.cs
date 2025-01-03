﻿using System;
using System.Reflection;

namespace SnivysUltimatePackage.API.ExternalTeams
{
    public class SerpentsHandCheck : ExternalTeamChecker
    {
        public override void Init(Assembly assembly)
        {
            PluginEnabled = true;

            Type mainClass = assembly.GetType("SerpentsHand.Plugin");
            Instance = mainClass.GetField("Instance").GetValue(null);
            FieldInfo = mainClass.GetField("IsSpawnable");
        }
    }
}