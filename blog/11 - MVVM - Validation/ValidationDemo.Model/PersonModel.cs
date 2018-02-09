using System.ComponentModel;
using Peregrine.WPF.Model;

namespace ValidationDemo.Model
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

    public class PersonModel : perModelBase
    {
        public PersonModel()
        {
            AddPropertyDependency(nameof(Age), nameof(SchoolName));
        }

        public string Name { get; set; }

        public AgeBand Age { get; set; }

        public string SchoolName { get; set; }

        protected override string ValidateProperty(string propertyName)
        {
            var result = base.ValidateProperty(propertyName);

            switch (propertyName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                        result = "Name is required";
                    break;
                case nameof(Age):
                    if (Age == AgeBand.AgeUnknown)
                        result = "Age is required";
                    break;
                case nameof(SchoolName):
                    if (Age >= AgeBand.Age6To10 && Age <= AgeBand.AgeOver18 && string.IsNullOrWhiteSpace(SchoolName))
                        result = "School Name is required";
                    break;
            }

            return result;
        }
    }
}