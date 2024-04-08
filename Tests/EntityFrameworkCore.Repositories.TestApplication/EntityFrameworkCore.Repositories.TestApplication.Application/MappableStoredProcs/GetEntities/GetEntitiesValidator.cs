using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEntitiesValidator : AbstractValidator<GetEntities>
    {
        [IntentManaged(Mode.Merge)]
        public GetEntitiesValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}