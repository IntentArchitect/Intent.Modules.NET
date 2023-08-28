using System;
using AzureFunctions.TestApplication.Application.Params.GetByIdsQueryTest;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Validators.Params.GetByIdsQueryTest
{
    public class GetByIdsQueryTestValidator : AbstractValidator<Application.Params.GetByIdsQueryTest.GetByIdsQueryTest>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public GetByIdsQueryTestValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Ids)
                .NotNull();
        }
    }
}