using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Supers.CreateSuper
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateSuperCommandValidator : AbstractValidator<CreateSuperCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateSuperCommandValidator()
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