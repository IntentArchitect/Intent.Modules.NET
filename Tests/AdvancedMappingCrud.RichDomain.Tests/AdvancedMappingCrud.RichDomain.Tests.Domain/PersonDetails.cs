using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain
{
    public class PersonDetails : ValueObject
    {
        public PersonDetails(Names name)
        {
            Name = name;
        }

        [IntentMerge]
        protected PersonDetails()
        {
            Name = null!;
        }

        public Names Name { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Name;
        }
    }
}