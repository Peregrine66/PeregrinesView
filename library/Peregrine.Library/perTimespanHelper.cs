using System;

namespace Peregrine.Library
{
    public static class perTimeSpanHelper
    {
        public static TimeSpan Forever { get; } = TimeSpan.FromMilliseconds(-1);

        public static bool IsForever(this TimeSpan timeSpan) => timeSpan.Equals(perTimeSpanHelper.Forever);
    }
}
