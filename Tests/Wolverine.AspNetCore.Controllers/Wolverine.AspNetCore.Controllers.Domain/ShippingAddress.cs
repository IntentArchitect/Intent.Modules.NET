using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Domain
{
    public class ShippingAddress : ValueObject
    {
        public ShippingAddress(string line1, string city, string postalCode, string country)
        {
            Line1 = line1;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }

        [IntentMerge]
        protected ShippingAddress()
        {
            Line1 = null!;
            City = null!;
            PostalCode = null!;
            Country = null!;
        }

        public string Line1 { get; private set; }
        public string City { get; private set; }
        public string PostalCode { get; private set; }
        public string Country { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Line1;
            yield return City;
            yield return PostalCode;
            yield return Country;
        }
    }
}