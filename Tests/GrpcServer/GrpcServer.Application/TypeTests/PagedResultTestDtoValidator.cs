using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application.TypeTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PagedResultTestDtoValidator : AbstractValidator<PagedResultTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public PagedResultTestDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PagedResultField)
                .NotNull();

            RuleFor(v => v.PagedResultFieldCollection)
                .NotNull();
        }
    }
}