using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Brands.DeactivateBrand
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeactivateBrandCommandValidator : AbstractValidator<DeactivateBrandCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeactivateBrandCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}