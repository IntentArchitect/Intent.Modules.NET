using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.GetOneToManySourceOneToManyDestById
{
    public class GetOneToManySourceOneToManyDestByIdQueryValidator : AbstractValidator<GetOneToManySourceOneToManyDestByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public GetOneToManySourceOneToManyDestByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}