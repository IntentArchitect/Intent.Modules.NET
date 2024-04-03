using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.MismatchType
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MismatchTypeQueryValidator : AbstractValidator<MismatchTypeQuery>
    {
        [IntentManaged(Mode.Merge)]
        public MismatchTypeQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}