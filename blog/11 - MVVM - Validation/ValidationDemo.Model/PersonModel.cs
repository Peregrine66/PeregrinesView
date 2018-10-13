using Peregrine.WPF.Model;
using ValidationDemo.Model.Enums;

namespace ValidationDemo.Model
{
    public class PersonModel : perModelBase
    {
        static PersonModel()
        {
            AddValidationDependency(nameof(Age), nameof(SchoolName));
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
                    if (Age >= AgeBand.Age6To10 && Age <= AgeBand.Age15To18 && string.IsNullOrWhiteSpace(SchoolName))
                        result = "School Name is required";
                    break;
            }

            return result;
        }
    }
}