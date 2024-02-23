using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateQuote
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateQuoteCommandValidator : AbstractValidator<CreateQuoteCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateQuoteCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefNo)
                .NotNull();
        }
    }
}