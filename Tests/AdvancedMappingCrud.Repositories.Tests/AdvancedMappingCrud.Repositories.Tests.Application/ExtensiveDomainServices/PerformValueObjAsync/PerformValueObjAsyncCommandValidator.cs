using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformValueObjAsync
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PerformValueObjAsyncCommandValidator : AbstractValidator<PerformValueObjAsyncCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PerformValueObjAsyncCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value1)
                .NotNull();
        }
    }
}