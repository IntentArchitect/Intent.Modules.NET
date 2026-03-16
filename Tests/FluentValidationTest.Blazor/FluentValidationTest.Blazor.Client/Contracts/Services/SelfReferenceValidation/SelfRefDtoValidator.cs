using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client.Contracts.Services.SelfReferenceValidation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SelfRefDtoValidator : AbstractValidator<SelfRefDto>
    {
        [IntentManaged(Mode.Merge)]
        public SelfRefDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Entry)
                .NotNull();

            RuleFor(v => v.SelfRefDtos)
                .NotNull()
                .ForEach(x => x.SetValidator(this));
        }
    }
}