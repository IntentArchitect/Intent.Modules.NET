using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.DeleteA_AggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteA_AggregateRootCommandValidator : AbstractValidator<DeleteA_AggregateRootCommand>
    {
        [IntentManaged(Mode.Fully)]
        public DeleteA_AggregateRootCommandValidator()
        {
        }
    }
}