using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.CreateWidget
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateWidgetCommandValidator : AbstractValidator<CreateWidgetCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateWidgetCommandValidator()
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