using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.CreateExplicitETag
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateExplicitETagCommandValidator : AbstractValidator<CreateExplicitETagCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateExplicitETagCommandValidator()
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