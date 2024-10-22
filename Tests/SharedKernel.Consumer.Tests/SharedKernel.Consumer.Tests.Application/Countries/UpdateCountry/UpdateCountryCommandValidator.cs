using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Countries.UpdateCountry
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCountryCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Code)
                .NotNull();
        }
    }
}