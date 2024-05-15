using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.AdvancedMappingSystem.Clients.UpdateClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.TagsIds)
                .NotNull();
        }
    }
}