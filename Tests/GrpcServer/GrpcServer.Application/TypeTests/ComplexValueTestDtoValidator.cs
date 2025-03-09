using FluentValidation;
using GrpcServer.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ComplexValueTestDtoValidator : AbstractValidator<ComplexValueTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public ComplexValueTestDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.ComplexTypeField)
                .NotNull()
                .SetValidator(provider.GetValidator<ComplexTypeDto>()!);

            RuleFor(v => v.ComplexTypeFieldCollection)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<ComplexTypeDto>()!));

            RuleFor(v => v.ComplexTypeFieldNullable)
                .SetValidator(provider.GetValidator<ComplexTypeDto>()!);

            RuleFor(v => v.ComplexTypeFieldNullableCollection)
                .ForEach(x => x.SetValidator(provider.GetValidator<ComplexTypeDto>()!));
        }
    }
}