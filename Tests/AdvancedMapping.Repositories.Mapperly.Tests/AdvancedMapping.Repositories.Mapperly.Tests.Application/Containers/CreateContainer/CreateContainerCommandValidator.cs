using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Containers.CreateContainer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateContainerCommandValidator : AbstractValidator<CreateContainerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateContainerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ContainerNumber)
                .NotNull();

            RuleFor(v => v.SealNumber)
                .NotNull();
        }
    }
}