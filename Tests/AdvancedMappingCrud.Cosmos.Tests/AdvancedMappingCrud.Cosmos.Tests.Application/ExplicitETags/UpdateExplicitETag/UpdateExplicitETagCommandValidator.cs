using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.UpdateExplicitETag
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateExplicitETagCommandValidator : AbstractValidator<UpdateExplicitETagCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateExplicitETagCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}