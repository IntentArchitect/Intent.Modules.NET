using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.PartialCruds.CreatePartialCrud
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePartialCrudCommandValidator : AbstractValidator<CreatePartialCrudCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePartialCrudCommandValidator()
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