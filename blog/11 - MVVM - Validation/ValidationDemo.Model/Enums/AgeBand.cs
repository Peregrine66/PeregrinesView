using System.ComponentModel;

namespace ValidationDemo.Model.Enums
{
    public enum AgeBand
    {
        [Description("")]
        AgeUnknown,
        [Description("Under 6")]
        AgeUnder6,
        [Description("6 - 10")]
        Age6To10,
        [Description("11 - 14")]
        Age11To14,
        [Description("15 - 18")]
        Age15To18,
        [Description("Over 18")]
        AgeOver18
    }
}