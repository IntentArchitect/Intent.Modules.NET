using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.DeleteAggregateTestNoIdReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteAggregateTestNoIdReturnCommandValidator : AbstractValidator<DeleteAggregateTestNoIdReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAggregateTestNoIdReturnCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}