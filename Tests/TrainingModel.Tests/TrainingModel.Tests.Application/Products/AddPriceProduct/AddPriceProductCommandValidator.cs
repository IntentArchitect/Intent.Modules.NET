using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Products.AddPriceProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AddPriceProductCommandValidator : AbstractValidator<AddPriceProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public AddPriceProductCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}