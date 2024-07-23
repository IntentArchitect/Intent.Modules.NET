using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Basics.CreateBasic
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBasicCommandValidator : AbstractValidator<CreateBasicCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBasicCommandValidator()
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