using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.UniqueIndexConstraint
{
    public class AggregateWithUniqueConstraintIndexStereotypeDto
    {
        public AggregateWithUniqueConstraintIndexStereotypeDto()
        {
            SingleUniqueField = null!;
            CompUniqueFieldA = null!;
            CompUniqueFieldB = null!;
        }

        public Guid Id { get; set; }
        public string SingleUniqueField { get; set; }
        public string CompUniqueFieldA { get; set; }
        public string CompUniqueFieldB { get; set; }

        public static AggregateWithUniqueConstraintIndexStereotypeDto Create(
            Guid id,
            string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB)
        {
            return new AggregateWithUniqueConstraintIndexStereotypeDto
            {
                Id = id,
                SingleUniqueField = singleUniqueField,
                CompUniqueFieldA = compUniqueFieldA,
                CompUniqueFieldB = compUniqueFieldB
            };
        }
    }
}