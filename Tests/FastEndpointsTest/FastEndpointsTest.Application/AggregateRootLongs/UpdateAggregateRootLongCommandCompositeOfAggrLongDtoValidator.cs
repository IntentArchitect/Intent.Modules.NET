using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateRootLongCommandCompositeOfAggrLongDtoValidator : AbstractValidator<UpdateAggregateRootLongCommandCompositeOfAggrLongDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootLongCommandCompositeOfAggrLongDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}