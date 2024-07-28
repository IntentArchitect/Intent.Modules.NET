using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.OpenApiIgnoreAllImplicit.OperationB
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OperationBValidator : AbstractValidator<OperationB>
    {
        [IntentManaged(Mode.Merge)]
        public OperationBValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}