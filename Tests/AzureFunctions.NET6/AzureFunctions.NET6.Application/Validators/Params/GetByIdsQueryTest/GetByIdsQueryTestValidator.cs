using System;
using AzureFunctions.NET6.Application.Params.GetByIdsQueryTest;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.Params.GetByIdsQueryTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetByIdsQueryTestValidator : AbstractValidator<Application.Params.GetByIdsQueryTest.GetByIdsQueryTest>
    {
        [IntentManaged(Mode.Merge)]
        public GetByIdsQueryTestValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Ids)
                .NotNull();
        }
    }
}