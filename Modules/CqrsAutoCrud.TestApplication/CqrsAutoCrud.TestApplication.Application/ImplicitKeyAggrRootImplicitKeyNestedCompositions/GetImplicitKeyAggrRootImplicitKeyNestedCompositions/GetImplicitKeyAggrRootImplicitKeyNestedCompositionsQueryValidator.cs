using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRootImplicitKeyNestedCompositions.GetImplicitKeyAggrRootImplicitKeyNestedCompositions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQueryValidator : AbstractValidator<GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQuery>
    {
        [IntentManaged(Mode.Fully)]
        public GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}