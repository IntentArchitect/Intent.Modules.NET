using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace EfCoreSoftDelete.Domain
{
    public class AddressBuilding : ValueObject
    {
        public AddressBuilding(string name)
        {
            Name = name;
        }

        [IntentMerge]
        protected AddressBuilding()
        {
            Name = null!;
        }

        public string Name { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}