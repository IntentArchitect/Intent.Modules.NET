using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Clients.GetClientById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}