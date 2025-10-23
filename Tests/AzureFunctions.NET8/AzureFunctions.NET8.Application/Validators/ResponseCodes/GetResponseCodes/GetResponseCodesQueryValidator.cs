using AzureFunctions.NET8.Application.ResponseCodes.GetResponseCodes;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.ResponseCodes.GetResponseCodes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetResponseCodesQueryValidator : AbstractValidator<GetResponseCodesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetResponseCodesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}