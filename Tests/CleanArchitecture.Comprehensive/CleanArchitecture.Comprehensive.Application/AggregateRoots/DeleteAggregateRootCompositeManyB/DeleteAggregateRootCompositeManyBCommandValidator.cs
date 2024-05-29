using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.DeleteAggregateRootCompositeManyB
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteAggregateRootCompositeManyBCommandValidator : AbstractValidator<DeleteAggregateRootCompositeManyBCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAggregateRootCompositeManyBCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}