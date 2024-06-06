using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.DeleteAggregateRootLong
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteAggregateRootLongCommandValidator : AbstractValidator<DeleteAggregateRootLongCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteAggregateRootLongCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}