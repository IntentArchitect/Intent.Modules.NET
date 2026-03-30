using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateUniqueAccountEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateUniqueAccountEntityCommandValidator : AbstractValidator<UpdateUniqueAccountEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateUniqueAccountEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Username)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}