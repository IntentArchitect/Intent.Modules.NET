using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.EfClients.UpdateEfClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEfClientCommandValidator : AbstractValidator<UpdateEfClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEfClientCommandValidator()
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