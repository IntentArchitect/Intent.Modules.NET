using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.DeleteGeometryType
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteGeometryTypeCommandValidator : AbstractValidator<DeleteGeometryTypeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteGeometryTypeCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}