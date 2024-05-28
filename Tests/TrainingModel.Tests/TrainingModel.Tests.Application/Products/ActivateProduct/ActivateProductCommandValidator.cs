using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Products.ActivateProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ActivateProductCommandValidator : AbstractValidator<ActivateProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ActivateProductCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}