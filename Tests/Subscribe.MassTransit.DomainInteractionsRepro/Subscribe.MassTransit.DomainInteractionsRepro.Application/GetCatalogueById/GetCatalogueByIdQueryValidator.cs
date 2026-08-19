using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application.GetCatalogueById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCatalogueByIdQueryValidator : AbstractValidator<GetCatalogueByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCatalogueByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}