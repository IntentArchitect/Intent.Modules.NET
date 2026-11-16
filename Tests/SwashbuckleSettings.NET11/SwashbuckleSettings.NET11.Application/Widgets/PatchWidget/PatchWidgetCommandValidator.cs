using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.PatchWidget
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchWidgetCommandValidator : AbstractValidator<PatchWidgetCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PatchWidgetCommandValidator()
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