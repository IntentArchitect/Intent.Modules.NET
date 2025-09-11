using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.EfClients.DeleteEfClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteEfClientCommandValidator : AbstractValidator<DeleteEfClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEfClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}