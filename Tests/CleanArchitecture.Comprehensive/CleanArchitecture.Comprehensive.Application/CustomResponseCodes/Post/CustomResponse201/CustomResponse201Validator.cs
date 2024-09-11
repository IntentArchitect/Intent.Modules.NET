using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse201
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse201Validator : AbstractValidator<CustomResponse201>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse201Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}