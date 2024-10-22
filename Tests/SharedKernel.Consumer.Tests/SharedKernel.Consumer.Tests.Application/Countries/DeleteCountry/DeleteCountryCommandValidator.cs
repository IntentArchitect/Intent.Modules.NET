using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Countries.DeleteCountry
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCountryCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}