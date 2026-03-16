using FluentValidation;
using FluentValidationTest.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation.UploadSelfRefDto
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UploadSelfRefDtoCommandValidator : AbstractValidator<UploadSelfRefDtoCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UploadSelfRefDtoCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Entry)
                .NotNull();

            RuleFor(v => v.SelfRefDtos)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<SelfRefDto>()!));
        }
    }
}