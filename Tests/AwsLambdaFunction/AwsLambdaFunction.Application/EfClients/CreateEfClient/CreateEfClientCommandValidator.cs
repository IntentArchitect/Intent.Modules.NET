using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.EfClients.CreateEfClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEfClientCommandValidator : AbstractValidator<CreateEfClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEfClientCommandValidator()
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