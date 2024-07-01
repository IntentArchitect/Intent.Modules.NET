using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.GetByTags
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetByTagsQueryValidator : AbstractValidator<GetByTagsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetByTagsQueryValidator()
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