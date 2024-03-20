using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetEntityName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntityNameValidator : AbstractValidator<GetEntityName>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntityNameValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}