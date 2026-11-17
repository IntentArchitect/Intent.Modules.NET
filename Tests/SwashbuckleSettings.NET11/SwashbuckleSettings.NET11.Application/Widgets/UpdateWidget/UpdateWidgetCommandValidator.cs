using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.UpdateWidget
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateWidgetCommandValidator : AbstractValidator<UpdateWidgetCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateWidgetCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}