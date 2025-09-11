using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynClients.CreateDynClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDynClientCommandValidator : AbstractValidator<CreateDynClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDynClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.AffiliateId)
                .NotNull();
        }
    }
}