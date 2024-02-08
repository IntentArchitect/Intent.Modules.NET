using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes
{
    public class CreateDerivedTypeDerivedTypeAggregateDto
    {
        public CreateDerivedTypeDerivedTypeAggregateDto()
        {
            AggregateName = null!;
        }

        public string AggregateName { get; set; }

        public static CreateDerivedTypeDerivedTypeAggregateDto Create(string aggregateName)
        {
            return new CreateDerivedTypeDerivedTypeAggregateDto
            {
                AggregateName = aggregateName
            };
        }
    }
}