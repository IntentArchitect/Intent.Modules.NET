using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.AdvancedMappingSystem.Clients.CreateClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.TagsIds)
                .NotNull();
        }
    }
}