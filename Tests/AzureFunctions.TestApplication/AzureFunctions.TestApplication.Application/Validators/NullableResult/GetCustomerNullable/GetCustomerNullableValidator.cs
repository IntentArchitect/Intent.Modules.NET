using System;
using AzureFunctions.TestApplication.Application.NullableResult.GetCustomerNullable;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Validators.NullableResult.GetCustomerNullable
{
    public class GetCustomerNullableValidator : AbstractValidator<Application.NullableResult.GetCustomerNullable.GetCustomerNullable>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public GetCustomerNullableValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}