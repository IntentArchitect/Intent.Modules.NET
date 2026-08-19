using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ComponentTypes.CreateComponentTypeProperty
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateComponentTypePropertyCommandValidator : AbstractValidator<CreateComponentTypePropertyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateComponentTypePropertyCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PropertyName)
                .NotNull();
        }
    }
}