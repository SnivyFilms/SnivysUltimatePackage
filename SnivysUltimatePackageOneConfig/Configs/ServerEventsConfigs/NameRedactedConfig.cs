﻿using System.ComponentModel;

namespace SnivysUltimatePackageOneConfig.Configs.ServerEventsConfigs
{
    public class NameRedactedConfig
    {
        public string StartEventCassieMessage { get; set; } = "Name Gone";
        public string StartEventCassieText { get; set; } = "Name Gone (Redacted Names Event)";

        [Description("What should the nickname of everyone be?")]
        public string NameRedactedName { get; set; } = "[REDACTED]";
    }
}