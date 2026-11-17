using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.SearchWidgets
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SearchWidgetsQueryValidator : AbstractValidator<SearchWidgetsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public SearchWidgetsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}