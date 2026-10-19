using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace FastEndpointsTest.Domain.DDD
{
    public class Address : ValueObject
    {
        public Address(string line1, string line2, string city, string postal)
        {
            Line1 = line1;
            Line2 = line2;
            City = city;
            Postal = postal;
        }

        [IntentMerge]
        protected Address()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Postal = null!;
        }

        public string Line1 { get; private set; }
        public string Line2 { get; private set; }
        public string City { get; private set; }
        public string Postal { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Line1;
            yield return Line2;
            yield return City;
            yield return Postal;
        }
    }
}