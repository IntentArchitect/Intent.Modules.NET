using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.CreateODataAgg
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateODataAggCommandValidator : AbstractValidator<CreateODataAggCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateODataAggCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}