using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.Clients.GetClientsByIds
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientsByIdsQueryValidator : AbstractValidator<GetClientsByIdsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientsByIdsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Ids)
                .NotNull();
        }
    }
}