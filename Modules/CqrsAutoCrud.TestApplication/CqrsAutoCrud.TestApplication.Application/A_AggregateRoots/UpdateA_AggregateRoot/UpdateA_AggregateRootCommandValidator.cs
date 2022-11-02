using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.UpdateA_AggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateA_AggregateRootCommandValidator : AbstractValidator<UpdateA_AggregateRootCommand>
    {
        [IntentManaged(Mode.Fully)]
        public UpdateA_AggregateRootCommandValidator()
        {
            RuleFor(v => v.AggregateAttr)
                .NotNull();

            RuleFor(v => v.Composites)
                .NotNull();

            RuleFor(v => v.Aggregations)
                .NotNull();

        }
    }
}