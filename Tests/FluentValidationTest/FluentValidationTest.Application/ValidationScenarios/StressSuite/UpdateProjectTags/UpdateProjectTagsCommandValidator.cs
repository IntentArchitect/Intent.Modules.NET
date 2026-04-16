using System.Text.RegularExpressions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.UpdateProjectTags
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateProjectTagsCommandValidator : AbstractValidator<UpdateProjectTagsCommand>
    {
        private static readonly Regex TagsRegex = new Regex(@"^[a-zA-Z0-9]*$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

        [IntentManaged(Mode.Merge)]
        public UpdateProjectTagsCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Tags)
                .NotNull()
                .ForEach(x => x.MaximumLength(15).Matches(TagsRegex));
        }
    }
}