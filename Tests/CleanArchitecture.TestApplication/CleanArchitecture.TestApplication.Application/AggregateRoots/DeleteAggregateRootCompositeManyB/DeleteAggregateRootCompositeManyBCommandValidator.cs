using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.DeleteAggregateRootCompositeManyB
{
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