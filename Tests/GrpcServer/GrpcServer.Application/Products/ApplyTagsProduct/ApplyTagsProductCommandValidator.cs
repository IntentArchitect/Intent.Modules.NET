using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace GrpcServer.Application.Products.ApplyTagsProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ApplyTagsProductCommandValidator : AbstractValidator<ApplyTagsProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ApplyTagsProductCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.TagNames)
                .NotNull();
        }
    }
}