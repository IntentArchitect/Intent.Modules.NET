using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes
{
    public class UpdateDerivedTypeDerivedTypeAggregateDto
    {
        public UpdateDerivedTypeDerivedTypeAggregateDto()
        {
            AggregateName = null!;
        }

        public string AggregateName { get; set; }

        public Guid Id { get; set; }

        public static UpdateDerivedTypeDerivedTypeAggregateDto Create(string aggregateName, Guid id)
        {
            return new UpdateDerivedTypeDerivedTypeAggregateDto
            {
                AggregateName = aggregateName,
                Id = id
            };
        }
    }
}