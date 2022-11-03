using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots.DeleteAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootCommandValidator : AbstractValidator<DeleteAggregateRootCommand>
    {
        [IntentManaged(Mode.Fully)]
        public DeleteAggregateRootCommandValidator()
        {
        }
    }
}