using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.ManyToMany.CreateProductItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateProductItemCommandValidator : AbstractValidator<CreateProductItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateProductItemCommandValidator()
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