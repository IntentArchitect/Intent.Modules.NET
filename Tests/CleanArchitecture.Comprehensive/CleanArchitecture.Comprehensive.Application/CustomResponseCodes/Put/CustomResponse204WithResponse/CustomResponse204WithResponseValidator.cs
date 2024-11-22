using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Put.CustomResponse204WithResponse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse204WithResponseValidator : AbstractValidator<CustomResponse204WithResponse>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse204WithResponseValidator()
        {
            ConfigureValidationRules();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Depends on user code")]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}