using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQueryValidator : AbstractValidator<GetImplicitKeyAggrRootImplicitKeyNestedCompositionsQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
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