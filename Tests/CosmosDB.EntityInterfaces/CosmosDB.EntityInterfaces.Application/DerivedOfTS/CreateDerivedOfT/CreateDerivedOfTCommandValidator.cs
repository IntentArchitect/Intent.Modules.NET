using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.DerivedOfTS.CreateDerivedOfT
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDerivedOfTCommandValidator : AbstractValidator<CreateDerivedOfTCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDerivedOfTCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DerivedAttribute)
                .NotNull();
        }
    }
}