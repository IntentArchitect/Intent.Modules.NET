using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace Application.Identity.AccountController.Domain
{
    public class Address : ValueObject
    {
        public Address(string line1, string line2)
        {
            Line1 = line1;
            Line2 = line2;
        }

        [IntentMerge]
        protected Address()
        {
            Line1 = null!;
            Line2 = null!;
        }

        public string Line1 { get; private set; }
        public string Line2 { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Line1;
            yield return Line2;
        }
    }
}