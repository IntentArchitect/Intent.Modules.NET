using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetEntityById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntityByIdValidator : AbstractValidator<GetEntityById>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntityByIdValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}