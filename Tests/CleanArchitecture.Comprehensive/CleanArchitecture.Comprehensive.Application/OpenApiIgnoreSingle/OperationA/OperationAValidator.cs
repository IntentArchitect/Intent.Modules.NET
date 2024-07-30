using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.OpenApiIgnoreSingle.OperationA
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OperationAValidator : AbstractValidator<OperationA>
    {
        [IntentManaged(Mode.Merge)]
        public OperationAValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}