using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.GetLogin
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetLoginQueryValidator : AbstractValidator<GetLoginQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetLoginQueryValidator()
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