using System;
using AzureFunctions.NET8.Application.NullableResult.GetCustomerNullable;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.NullableResult.GetCustomerNullable
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerNullableValidator : AbstractValidator<Application.NullableResult.GetCustomerNullable.GetCustomerNullable>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetCustomerNullableValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}