using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.ActivateBuyer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ActivateBuyerCommandValidator : AbstractValidator<ActivateBuyerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ActivateBuyerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}