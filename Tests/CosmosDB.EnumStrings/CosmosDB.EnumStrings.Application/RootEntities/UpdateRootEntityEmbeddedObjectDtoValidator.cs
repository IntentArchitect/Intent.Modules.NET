using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateRootEntityEmbeddedObjectDtoValidator : AbstractValidator<UpdateRootEntityEmbeddedObjectDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateRootEntityEmbeddedObjectDtoValidator()
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