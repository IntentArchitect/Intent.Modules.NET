using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.GetUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Username)
                .NotNull();
        }
    }
}