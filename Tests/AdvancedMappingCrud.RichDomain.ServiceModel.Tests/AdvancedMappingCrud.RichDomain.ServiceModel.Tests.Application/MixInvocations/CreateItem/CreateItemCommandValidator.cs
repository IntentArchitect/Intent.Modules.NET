using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.MixInvocations.CreateItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateItemCommandValidator()
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