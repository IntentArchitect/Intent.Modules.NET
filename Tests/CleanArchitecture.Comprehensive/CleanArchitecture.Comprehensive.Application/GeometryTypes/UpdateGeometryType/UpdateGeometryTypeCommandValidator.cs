using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.UpdateGeometryType
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateGeometryTypeCommandValidator : AbstractValidator<UpdateGeometryTypeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateGeometryTypeCommandValidator()
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