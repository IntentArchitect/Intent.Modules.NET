using FluentValidation;
using GrpcServer.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace GrpcServer.Application.Products.CreateComplexProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateComplexProductCommandValidator : AbstractValidator<CreateComplexProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateComplexProductCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.TypeTestField)
                .NotNull()
                .SetValidator(provider.GetValidator<TypeTestDto>()!);
        }
    }
}