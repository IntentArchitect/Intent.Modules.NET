using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.CreateGeometryType
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateGeometryTypeCommandValidator : AbstractValidator<CreateGeometryTypeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateGeometryTypeCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Point)
                .NotNull();
        }
    }
}