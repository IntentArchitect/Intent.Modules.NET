using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace OutputCachingRedis.Tests.Application.Files.CreateFiles
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateFilesCommandValidator : AbstractValidator<CreateFilesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateFilesCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Content)
                .NotNull();
        }
    }
}