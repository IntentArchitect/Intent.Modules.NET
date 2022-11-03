using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.DeleteAggregateRootLong
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootLongCommandValidator : AbstractValidator<DeleteAggregateRootLongCommand>
    {
        [IntentManaged(Mode.Fully)]
        public DeleteAggregateRootLongCommandValidator()
        {
        }
    }
}