using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Delete.CustomResponse208WithResponse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse208WithResponseValidator : AbstractValidator<CustomResponse208WithResponse>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse208WithResponseValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}