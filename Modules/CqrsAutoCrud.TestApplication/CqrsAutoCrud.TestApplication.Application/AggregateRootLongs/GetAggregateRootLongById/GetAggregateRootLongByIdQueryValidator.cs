using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootLongByIdQueryValidator : AbstractValidator<GetAggregateRootLongByIdQuery>
    {
        [IntentManaged(Mode.Fully)]
        public GetAggregateRootLongByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}