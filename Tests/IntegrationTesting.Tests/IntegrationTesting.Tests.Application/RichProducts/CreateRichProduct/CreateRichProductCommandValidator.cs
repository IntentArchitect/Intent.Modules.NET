using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.CreateRichProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateRichProductCommandValidator : AbstractValidator<CreateRichProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateRichProductCommandValidator()
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