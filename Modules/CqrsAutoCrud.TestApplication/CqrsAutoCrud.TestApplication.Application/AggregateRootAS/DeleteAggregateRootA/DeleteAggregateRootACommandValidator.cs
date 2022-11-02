using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS.DeleteAggregateRootA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootACommandValidator : AbstractValidator<DeleteAggregateRootACommand>
    {
        [IntentManaged(Mode.Fully)]
        public DeleteAggregateRootACommandValidator()
        {
        }
    }
}