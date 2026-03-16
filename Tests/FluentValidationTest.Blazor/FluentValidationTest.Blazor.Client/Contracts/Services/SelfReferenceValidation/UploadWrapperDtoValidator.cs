using FluentValidation;
using FluentValidationTest.Blazor.Client.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client.Contracts.Services.SelfReferenceValidation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UploadWrapperDtoValidator : AbstractValidator<UploadWrapperDto>
    {
        [IntentManaged(Mode.Merge)]
        public UploadWrapperDtoValidator(IValidatorProvider provider)
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