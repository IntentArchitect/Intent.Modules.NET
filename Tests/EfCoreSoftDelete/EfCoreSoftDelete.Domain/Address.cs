using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace EfCoreSoftDelete.Domain
{
    public class Address : ValueObject
    {
        public Address(string line1,
            string line2,
            string city,
            string postal,
            IEnumerable<AddressBuilding> otherBuildings,
            AddressBuilding primaryBuilding)
        {
            Line1 = line1;
            Line2 = line2;
            City = city;
            Postal = postal;
            OtherBuildings = otherBuildings;
            PrimaryBuilding = primaryBuilding;
        }

        [IntentMerge]
        protected Address()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Postal = null!;
            OtherBuildings = null!;
            PrimaryBuilding = null!;
        }

        public string Line1 { get; private set; }
        public string Line2 { get; private set; }
        public string City { get; private set; }
        public string Postal { get; private set; }
        public IEnumerable<AddressBuilding> OtherBuildings { get; private set; }
        public AddressBuilding PrimaryBuilding { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Line1;
            yield return Line2;
            yield return City;
            yield return Postal;
            yield return OtherBuildings;
            yield return PrimaryBuilding;
        }
    }
}