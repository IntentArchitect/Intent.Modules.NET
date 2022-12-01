using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRootImplicitKeyNestedCompositions.GetImplicitKeyAggrRootImplicitKeyNestedCompositionById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryValidator : AbstractValidator<GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery>
    {
        [IntentManaged(Mode.Fully)]
        public GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}