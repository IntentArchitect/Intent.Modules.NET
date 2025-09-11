using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynClients.GetDynClientById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDynClientByIdQueryValidator : AbstractValidator<GetDynClientByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDynClientByIdQueryValidator()
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