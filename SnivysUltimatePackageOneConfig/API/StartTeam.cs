﻿using System;

namespace SnivysUltimatePackageOneConfig.API
{
    [Flags]
    public enum StartTeam
    {
        ClassD = 1,
        Scientist = 2,
        Guard = 4,
        Ntf = 8,
        Chaos = 16,
        Scp = 32,
        Revived = 64,
        Escape = 128,
        Other = 256,
    }
}