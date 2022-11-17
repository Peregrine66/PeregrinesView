using System;
using System.ComponentModel;

namespace FlagsCheckBoxBindingDemo.Enums
{
    [Flags]
    public enum DemoFlagsEnum1
    {
        [Description("None")]
        Enum1None = 0,
        A1 = 1,
        B1 = 1 << 1,
        C1 = 1 << 2,
        D1 = 1 << 3,
        E1 = 1 << 4,
        [Description("All")]
        Enum1GroupAll = A1 | B1 | C1 | D1 | E1
    }
}
