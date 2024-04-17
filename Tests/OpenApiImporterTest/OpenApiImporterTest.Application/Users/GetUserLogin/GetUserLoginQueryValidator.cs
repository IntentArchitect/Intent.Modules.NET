using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.GetUserLogin
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUserLoginQueryValidator : AbstractValidator<GetUserLoginQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserLoginQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Username)
                .NotNull();

            RuleFor(v => v.Password)
                .NotNull();
        }
    }
}