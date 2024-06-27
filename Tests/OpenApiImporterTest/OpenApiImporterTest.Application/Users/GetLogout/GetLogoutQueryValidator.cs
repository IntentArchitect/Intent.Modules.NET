using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.GetLogout
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetLogoutQueryValidator : AbstractValidator<GetLogoutQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetLogoutQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}