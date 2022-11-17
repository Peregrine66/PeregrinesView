using System;
using System.ComponentModel;

namespace FlagsCheckBoxBindingDemo.Enums
{
    [Flags]
    public enum DemoFlagsEnum2
    {
        [Description("None")]
        Enum2None = 0,
        A2 = 1,
        B2 = 1 << 1,
        C2 = 1 << 2,
        D2 = 1 << 3,
        E2 = 1 << 4,
        F2 = 1 << 5,
        [Description("All")]
        Enum2GroupAll = A2 | B2 | C2 | D2 | E2 | F2,
        [Description("A2 and B2")]
        Enum2GroupA2AndB2 = A2 | B2,
        [Description("D2 and E2")]
        Enum2GroupD2AndE2 = D2 | E2
    }


}