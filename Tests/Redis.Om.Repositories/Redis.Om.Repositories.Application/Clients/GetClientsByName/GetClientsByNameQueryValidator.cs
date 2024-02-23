using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.Clients.GetClientsByName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientsByNameQueryValidator : AbstractValidator<GetClientsByNameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientsByNameQueryValidator()
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