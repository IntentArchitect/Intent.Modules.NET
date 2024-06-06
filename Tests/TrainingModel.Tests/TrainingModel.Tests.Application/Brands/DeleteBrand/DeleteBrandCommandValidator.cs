using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Brands.DeleteBrand
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteBrandCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}