using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateProductTagDtoValidator : AbstractValidator<UpdateProductTagDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateProductTagDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}