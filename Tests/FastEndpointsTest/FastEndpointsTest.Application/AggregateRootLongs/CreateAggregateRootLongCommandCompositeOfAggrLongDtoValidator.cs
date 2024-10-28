using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateRootLongCommandCompositeOfAggrLongDtoValidator : AbstractValidator<CreateAggregateRootLongCommandCompositeOfAggrLongDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootLongCommandCompositeOfAggrLongDtoValidator()
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