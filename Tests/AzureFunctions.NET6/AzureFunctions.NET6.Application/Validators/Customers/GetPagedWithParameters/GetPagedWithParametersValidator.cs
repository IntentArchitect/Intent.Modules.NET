using System;
using AzureFunctions.NET6.Application.Customers.GetPagedWithParameters;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.Customers.GetPagedWithParameters
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPagedWithParametersValidator : AbstractValidator<Application.Customers.GetPagedWithParameters.GetPagedWithParameters>
    {
        [IntentManaged(Mode.Merge)]
        public GetPagedWithParametersValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SearchCriteria)
                .NotNull();
        }
    }
}