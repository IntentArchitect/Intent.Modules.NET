using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.CreateCustomerSegments
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCustomerSegmentsCommandValidator : AbstractValidator<CreateCustomerSegmentsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCustomerSegmentsCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ClassificationSource)
                .NotNull()
                .IsInEnum();
        }
    }
}