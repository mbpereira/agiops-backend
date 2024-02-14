﻿using System.Text.RegularExpressions;

namespace PlanningPoker.Domain.Shared
{
    public static partial class StringPatterns
    {
        [GeneratedRegex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$")]
        public static partial Regex Email();
    }
}
