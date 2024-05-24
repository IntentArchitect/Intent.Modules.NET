using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Products
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeCategoriesSyncDtoValidator : AbstractValidator<ChangeCategoriesSyncDto>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeCategoriesSyncDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CategoryNames)
                .NotNull();
        }
    }
}