using CosmosDB.EnumStrings.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.CreateRootEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateRootEntityCommandValidator : AbstractValidator<CreateRootEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateRootEntityCommandValidator(IValidatorProvider provider)
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
                .SetValidator(provider.GetValidator<CreateRootEntityEmbeddedObjectDto>()!);

            RuleFor(v => v.NestedEntities)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateRootEntityCommandNestedEntitiesDto>()!));
        }
    }
}