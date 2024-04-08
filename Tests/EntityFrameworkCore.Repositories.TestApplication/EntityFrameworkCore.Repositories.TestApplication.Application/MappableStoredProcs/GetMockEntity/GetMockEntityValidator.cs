using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetMockEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetMockEntityValidator : AbstractValidator<GetMockEntity>
    {
        [IntentManaged(Mode.Merge)]
        public GetMockEntityValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}