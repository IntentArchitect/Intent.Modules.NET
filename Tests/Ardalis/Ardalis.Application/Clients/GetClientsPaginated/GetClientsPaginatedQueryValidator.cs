using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Ardalis.Application.Clients.GetClientsPaginated
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientsPaginatedQueryValidator : AbstractValidator<GetClientsPaginatedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientsPaginatedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}