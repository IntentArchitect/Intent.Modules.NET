using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases.GetPurchases
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPurchasesQueryValidator : AbstractValidator<GetPurchasesQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetPurchasesQueryValidator()
        {
            ConfigureValidationRules();

        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}