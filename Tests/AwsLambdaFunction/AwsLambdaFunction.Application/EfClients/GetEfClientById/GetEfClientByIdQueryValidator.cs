using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.EfClients.GetEfClientById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEfClientByIdQueryValidator : AbstractValidator<GetEfClientByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEfClientByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}