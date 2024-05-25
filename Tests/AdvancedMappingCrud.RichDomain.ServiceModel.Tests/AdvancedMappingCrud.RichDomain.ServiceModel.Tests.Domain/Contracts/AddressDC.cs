using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts
{
    public record AddressDC
    {
        public AddressDC(string line1, string line2, string city, int postal)
        {
            Line1 = line1;
            Line2 = line2;
            City = city;
            Postal = postal;
        }

        public string Line1 { get; init; }
        public string Line2 { get; init; }
        public string City { get; init; }
        public int Postal { get; init; }
    }
}