using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace EventingSubscribers.Domain
{
    public class Address : ValueObject
    {
        public Address(string street, string city)
        {
            Street = street;
            City = city;
        }

        [IntentMerge]
        protected Address()
        {
            Street = null!;
            City = null!;
        }

        public string Street { get; private set; }
        public string City { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street;
            yield return City;
        }
    }
}