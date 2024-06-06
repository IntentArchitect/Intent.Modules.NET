using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.CreateAggregateTestNoIdReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateTestNoIdReturnCommandValidator : AbstractValidator<CreateAggregateTestNoIdReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateTestNoIdReturnCommandValidator()
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