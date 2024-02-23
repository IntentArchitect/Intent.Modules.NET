using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.PartialCruds.UpdatePartialCrud
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePartialCrudCommandValidator : AbstractValidator<UpdatePartialCrudCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePartialCrudCommandValidator()
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