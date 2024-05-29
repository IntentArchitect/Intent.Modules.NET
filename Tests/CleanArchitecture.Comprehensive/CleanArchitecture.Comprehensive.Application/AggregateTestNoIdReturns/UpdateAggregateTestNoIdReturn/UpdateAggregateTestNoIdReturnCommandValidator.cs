using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.UpdateAggregateTestNoIdReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateTestNoIdReturnCommandValidator : AbstractValidator<UpdateAggregateTestNoIdReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateTestNoIdReturnCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}