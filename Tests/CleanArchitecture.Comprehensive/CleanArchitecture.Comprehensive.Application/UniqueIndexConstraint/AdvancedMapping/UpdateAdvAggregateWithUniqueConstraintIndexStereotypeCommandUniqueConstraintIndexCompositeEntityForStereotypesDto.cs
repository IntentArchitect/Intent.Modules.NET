using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping
{
    public class UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto
    {
        public UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto()
        {
            Field = null!;
        }

        public Guid Id { get; set; }
        public string Field { get; set; }

        public static UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto Create(
            Guid id,
            string field)
        {
            return new UpdateAdvAggregateWithUniqueConstraintIndexStereotypeCommandUniqueConstraintIndexCompositeEntityForStereotypesDto
            {
                Id = id,
                Field = field
            };
        }
    }
}