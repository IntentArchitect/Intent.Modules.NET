using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.Clients.GetClientsPaged
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientsPagedQueryValidator : AbstractValidator<GetClientsPagedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientsPagedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}