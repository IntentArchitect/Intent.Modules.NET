using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.DerivedOfTS.DeleteDerivedOfT
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDerivedOfTCommandValidator : AbstractValidator<DeleteDerivedOfTCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDerivedOfTCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}