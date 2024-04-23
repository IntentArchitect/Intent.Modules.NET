using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.GetPetFindByStatus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPetFindByStatusQueryValidator : AbstractValidator<GetPetFindByStatusQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPetFindByStatusQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Status)
                .NotNull();
        }
    }
}