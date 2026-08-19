using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Subscribe.MassTransit.DomainInteractionsRepro.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application.CreateCatalogue
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCatalogueCommandValidator : AbstractValidator<CreateCatalogueCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCatalogueCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Code)
                .NotNull();

            RuleFor(v => v.CatalogueItems)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateCatalogueItemDto>()!));
        }
    }
}