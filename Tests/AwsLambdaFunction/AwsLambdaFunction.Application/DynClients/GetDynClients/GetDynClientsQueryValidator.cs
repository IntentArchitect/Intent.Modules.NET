using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynClients.GetDynClients
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDynClientsQueryValidator : AbstractValidator<GetDynClientsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDynClientsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}