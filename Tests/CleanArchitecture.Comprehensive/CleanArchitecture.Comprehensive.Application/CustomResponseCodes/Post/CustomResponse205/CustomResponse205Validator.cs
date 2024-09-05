using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse205
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomResponse205Validator : AbstractValidator<CustomResponse205>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse205Validator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}