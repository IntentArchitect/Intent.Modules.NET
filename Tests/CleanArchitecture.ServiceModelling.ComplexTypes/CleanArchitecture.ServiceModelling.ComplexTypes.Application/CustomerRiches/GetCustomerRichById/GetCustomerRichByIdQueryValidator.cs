using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.GetCustomerRichById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerRichByIdQueryValidator : AbstractValidator<GetCustomerRichByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetCustomerRichByIdQueryValidator()
        {
            ConfigureValidationRules();

        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}