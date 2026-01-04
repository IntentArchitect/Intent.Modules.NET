using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ManyToMany
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateProductItemDtoValidator : AbstractValidator<UpdateProductItemDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateProductItemDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.TagIds)
                .NotNull();

            RuleFor(v => v.CategoryIds)
                .NotNull();
        }
    }
}