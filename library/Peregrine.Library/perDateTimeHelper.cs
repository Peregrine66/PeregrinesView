using System;

namespace Peregrine.Library
{
    public static class perDateTimeHelper
    {
        public static int DateToInt(this DateTime dt)
        {
            return 10000 * dt.Year + 100 * dt.Month + dt.Day;
        }

        public static int TimeToInt(this DateTime dt)
        {
            return 10000000 * dt.Hour + 100000 * dt.Minute + 1000 * dt.Second + dt.Millisecond;
        }
    }
}
