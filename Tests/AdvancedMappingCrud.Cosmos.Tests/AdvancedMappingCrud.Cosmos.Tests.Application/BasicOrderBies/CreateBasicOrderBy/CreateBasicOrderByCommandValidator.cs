using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.CreateBasicOrderBy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBasicOrderByCommandValidator : AbstractValidator<CreateBasicOrderByCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBasicOrderByCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();
        }
    }
}