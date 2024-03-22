using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformPassthroughAsync
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PerformPassthroughAsyncCommandValidator : AbstractValidator<PerformPassthroughAsyncCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PerformPassthroughAsyncCommandValidator()
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