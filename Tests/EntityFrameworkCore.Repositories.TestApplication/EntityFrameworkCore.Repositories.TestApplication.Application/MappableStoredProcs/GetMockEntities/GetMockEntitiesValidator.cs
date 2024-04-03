using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetMockEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetMockEntitiesValidator : AbstractValidator<GetMockEntities>
    {
        [IntentManaged(Mode.Merge)]
        public GetMockEntitiesValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}