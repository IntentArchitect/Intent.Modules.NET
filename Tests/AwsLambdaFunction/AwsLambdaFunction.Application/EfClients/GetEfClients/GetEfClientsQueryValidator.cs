using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.EfClients.GetEfClients
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEfClientsQueryValidator : AbstractValidator<GetEfClientsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEfClientsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}