using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.CreateCheckNewCompChildCrud
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCheckNewCompChildCrudCommandValidator : AbstractValidator<CreateCheckNewCompChildCrudCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCheckNewCompChildCrudCommandValidator()
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