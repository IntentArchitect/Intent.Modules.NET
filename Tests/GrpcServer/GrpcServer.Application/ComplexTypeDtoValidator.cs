using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ComplexTypeDtoValidator : AbstractValidator<ComplexTypeDto>
    {
        [IntentManaged(Mode.Merge)]
        public ComplexTypeDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field)
                .NotNull();
        }
    }
}