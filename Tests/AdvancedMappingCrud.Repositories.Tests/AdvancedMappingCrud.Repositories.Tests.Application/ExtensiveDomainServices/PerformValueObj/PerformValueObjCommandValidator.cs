using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformValueObj
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PerformValueObjCommandValidator : AbstractValidator<PerformValueObjCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PerformValueObjCommandValidator()
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