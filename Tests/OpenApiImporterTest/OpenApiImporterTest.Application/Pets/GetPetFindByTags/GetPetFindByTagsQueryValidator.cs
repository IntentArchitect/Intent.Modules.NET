using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.GetPetFindByTags
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPetFindByTagsQueryValidator : AbstractValidator<GetPetFindByTagsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPetFindByTagsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Tags)
                .NotNull();
        }
    }
}