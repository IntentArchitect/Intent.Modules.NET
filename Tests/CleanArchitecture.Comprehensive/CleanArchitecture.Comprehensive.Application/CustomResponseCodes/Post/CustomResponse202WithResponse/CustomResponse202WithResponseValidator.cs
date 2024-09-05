using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse202WithResponse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse202WithResponseValidator : AbstractValidator<CustomResponse202WithResponse>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse202WithResponseValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}