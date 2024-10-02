using CosmosDB.EnumStrings.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.UpdateRootEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateRootEntityCommandValidator : AbstractValidator<UpdateRootEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateRootEntityCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.EnumExample)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.NullableEnumExample)
                .IsInEnum();

            RuleFor(v => v.Embedded)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateRootEntityEmbeddedObjectDto>()!);

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}