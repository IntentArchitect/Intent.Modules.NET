using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Segments.CreateSegment
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateSegmentCommandValidator : AbstractValidator<CreateSegmentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateSegmentCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SegmentType)
                .NotNull()
                .IsInEnum();
        }
    }
}