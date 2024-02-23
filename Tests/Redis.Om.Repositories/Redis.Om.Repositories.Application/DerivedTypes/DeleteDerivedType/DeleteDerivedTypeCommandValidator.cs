using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.DeleteDerivedType
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDerivedTypeCommandValidator : AbstractValidator<DeleteDerivedTypeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDerivedTypeCommandValidator()
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