using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.GetUserLogout
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUserLogoutQueryValidator : AbstractValidator<GetUserLogoutQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserLogoutQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}