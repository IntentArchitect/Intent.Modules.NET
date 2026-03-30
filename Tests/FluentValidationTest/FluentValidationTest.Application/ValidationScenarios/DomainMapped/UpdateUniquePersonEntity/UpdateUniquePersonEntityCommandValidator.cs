using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateUniquePersonEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateUniquePersonEntityCommandValidator : AbstractValidator<UpdateUniquePersonEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateUniquePersonEntityCommandValidator()
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