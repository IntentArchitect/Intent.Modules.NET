using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynClients.UpdateDynClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDynClientCommandValidator : AbstractValidator<UpdateDynClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDynClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.AffiliateId)
                .NotNull();
        }
    }
}