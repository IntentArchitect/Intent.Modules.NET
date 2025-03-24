using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Contracts
{
    public record AddressDC
    {
        public AddressDC(string line1, string line2, string city)
        {
            Line1 = line1;
            Line2 = line2;
            City = city;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected AddressDC()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }

        public string Line1 { get; init; }
        public string Line2 { get; init; }
        public string City { get; init; }
    }
}