using System;
using AzureFunctions.TestApplication.Application.Customers.GetPagedWithParameters;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.Validators.Customers.GetPagedWithParameters
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPagedWithParametersValidator : AbstractValidator<Application.Customers.GetPagedWithParameters.GetPagedWithParameters>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetPagedWithParametersValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SearchCriteria)
                .NotNull();
        }
    }
}