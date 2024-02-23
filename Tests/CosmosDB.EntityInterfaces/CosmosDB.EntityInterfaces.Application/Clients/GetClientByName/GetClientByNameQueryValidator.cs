using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.Clients.GetClientByName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientByNameQueryValidator : AbstractValidator<GetClientByNameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientByNameQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SearchText)
                .NotNull();
        }
    }
}