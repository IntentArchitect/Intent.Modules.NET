using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.DerivedOfTS.UpdateDerivedOfT
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDerivedOfTCommandValidator : AbstractValidator<UpdateDerivedOfTCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public UpdateDerivedOfTCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.DerivedAttribute)
                .NotNull();
        }
    }
}