using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynClients.DeleteDynClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDynClientCommandValidator : AbstractValidator<DeleteDynClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDynClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}