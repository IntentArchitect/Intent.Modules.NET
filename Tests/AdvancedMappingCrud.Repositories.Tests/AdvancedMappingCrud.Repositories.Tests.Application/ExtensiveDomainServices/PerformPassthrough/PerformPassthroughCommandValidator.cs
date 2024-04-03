using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformPassthrough
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PerformPassthroughCommandValidator : AbstractValidator<PerformPassthroughCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PerformPassthroughCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ConcreteAttr)
                .NotNull();

            RuleFor(v => v.BaseAttr)
                .NotNull();
        }
    }
}