using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace MudBlazor.Sample.Domain
{
    public class Address : ValueObject
    {
        public Address(string line1, string? line2, string city, string country, string postal)
        {
            Line1 = line1;
            Line2 = line2;
            City = city;
            Country = country;
            Postal = postal;
        }

        [IntentMerge]
        protected Address()
        {
            Line1 = null!;
            City = null!;
            Country = null!;
            Postal = null!;
        }

        public string Line1 { get; private set; }
        public string? Line2 { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string Postal { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Line1;
            yield return Line2;
            yield return City;
            yield return Country;
            yield return Postal;
        }
    }
}