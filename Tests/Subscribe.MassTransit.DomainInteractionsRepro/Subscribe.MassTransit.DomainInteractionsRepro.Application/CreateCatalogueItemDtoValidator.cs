using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCatalogueItemDtoValidator : AbstractValidator<CreateCatalogueItemDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCatalogueItemDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}