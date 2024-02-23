using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.NoReturns.UpdateNoReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateNoReturnCommandValidator : AbstractValidator<UpdateNoReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateNoReturnCommandValidator()
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