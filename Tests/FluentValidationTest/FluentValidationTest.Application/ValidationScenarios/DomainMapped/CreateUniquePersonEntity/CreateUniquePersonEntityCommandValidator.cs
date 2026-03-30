using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateUniquePersonEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUniquePersonEntityCommandValidator : AbstractValidator<CreateUniquePersonEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUniquePersonEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FirstName)
                .NotNull();

            RuleFor(v => v.LastName)
                .NotNull();
        }
    }
}