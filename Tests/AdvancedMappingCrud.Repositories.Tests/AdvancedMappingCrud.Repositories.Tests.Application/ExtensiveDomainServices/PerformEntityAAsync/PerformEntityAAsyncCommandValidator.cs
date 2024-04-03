using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformEntityAAsync
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PerformEntityAAsyncCommandValidator : AbstractValidator<PerformEntityAAsyncCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PerformEntityAAsyncCommandValidator()
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