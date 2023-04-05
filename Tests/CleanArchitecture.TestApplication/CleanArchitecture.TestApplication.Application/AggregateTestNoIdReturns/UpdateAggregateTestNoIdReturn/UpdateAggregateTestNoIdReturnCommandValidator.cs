using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.UpdateAggregateTestNoIdReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateTestNoIdReturnCommandValidator : AbstractValidator<UpdateAggregateTestNoIdReturnCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
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