using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformEntityB
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PerformEntityBCommandValidator : AbstractValidator<PerformEntityBCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PerformEntityBCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.BaseAttr)
                .NotNull();

            RuleFor(v => v.ConcreteAttr)
                .NotNull();
        }
    }
}