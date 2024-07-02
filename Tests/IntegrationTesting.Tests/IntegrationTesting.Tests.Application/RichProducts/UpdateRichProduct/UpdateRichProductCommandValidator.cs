using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.UpdateRichProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateRichProductCommandValidator : AbstractValidator<UpdateRichProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateRichProductCommandValidator()
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