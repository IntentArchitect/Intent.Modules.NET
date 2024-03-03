using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources.GetOneToOptionalSourceById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOneToOptionalSourceByIdQueryValidator : AbstractValidator<GetOneToOptionalSourceByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetOneToOptionalSourceByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}