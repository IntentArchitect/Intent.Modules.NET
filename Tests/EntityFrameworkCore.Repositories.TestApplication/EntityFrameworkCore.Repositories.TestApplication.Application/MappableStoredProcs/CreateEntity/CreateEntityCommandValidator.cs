using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.CreateEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEntityCommandValidator : AbstractValidator<CreateEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}