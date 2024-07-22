using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using OpenApiImporterTest.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.CreatePet
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePetCommandValidator : AbstractValidator<CreatePetCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePetCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Category)
                .SetValidator(provider.GetValidator<Category>()!);

            RuleFor(v => v.PhotoUrls)
                .NotNull();

            RuleFor(v => v.Tags)
                .ForEach(x => x.SetValidator(provider.GetValidator<Tag>()!));

            RuleFor(v => v.Status)
                .IsInEnum();
        }
    }
}