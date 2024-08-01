using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.ValueObjects
{
    public class AddressNormal : ValueObject
    {
        protected AddressNormal()
        {
        }

        public AddressNormal(string line1, string line2, string city)
        {
            Line1 = line1;
            Line2 = line2;
            City = city;
        }

        public string Line1 { get; private set; }
        public string Line2 { get; private set; }
        public string City { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Line1;
            yield return Line2;
            yield return City;
        }
    }
}