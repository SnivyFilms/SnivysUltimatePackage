﻿namespace SnivysFreeCustomRoles
{
    public interface ICustomRole
    {
        public StartTeam StartTeam { get; set; }

        public int Chance { get; set; }
    }
}