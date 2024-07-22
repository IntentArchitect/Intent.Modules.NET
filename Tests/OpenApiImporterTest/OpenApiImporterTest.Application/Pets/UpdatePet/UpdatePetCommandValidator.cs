using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using OpenApiImporterTest.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.UpdatePet
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePetCommandValidator : AbstractValidator<UpdatePetCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePetCommandValidator(IValidatorProvider provider)
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