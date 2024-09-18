using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateRootEntityEmbeddedObjectDtoValidator : AbstractValidator<CreateRootEntityEmbeddedObjectDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateRootEntityEmbeddedObjectDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.EnumExample)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.NullableEnumExample)
                .NotNull()
                .IsInEnum();
        }
    }
}