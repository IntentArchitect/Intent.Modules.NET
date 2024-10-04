using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class Address
    {
        public Address(string line1, string line2, string city, int postal)
        {
            Line1 = line1;
            Line2 = line2;
            City = city;
            Postal = postal;
        }
    }
}