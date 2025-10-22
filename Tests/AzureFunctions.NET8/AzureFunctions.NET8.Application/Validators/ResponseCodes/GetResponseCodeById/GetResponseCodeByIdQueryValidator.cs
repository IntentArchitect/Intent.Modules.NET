using AzureFunctions.NET8.Application.ResponseCodes.GetResponseCodeById;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.ResponseCodes.GetResponseCodeById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetResponseCodeByIdQueryValidator : AbstractValidator<GetResponseCodeByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetResponseCodeByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}