using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.Clients.GetClientByName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientByNameValidator : AbstractValidator<GetClientByName>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientByNameValidator()
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