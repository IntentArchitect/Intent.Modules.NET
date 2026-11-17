using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.UploadWidgetImage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UploadWidgetImageCommandValidator : AbstractValidator<UploadWidgetImageCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UploadWidgetImageCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Content)
                .NotNull();
        }
    }
}