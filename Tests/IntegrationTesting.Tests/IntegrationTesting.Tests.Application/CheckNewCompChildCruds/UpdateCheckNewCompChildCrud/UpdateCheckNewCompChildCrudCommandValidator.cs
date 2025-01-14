using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.UpdateCheckNewCompChildCrud
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCheckNewCompChildCrudCommandValidator : AbstractValidator<UpdateCheckNewCompChildCrudCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCheckNewCompChildCrudCommandValidator()
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