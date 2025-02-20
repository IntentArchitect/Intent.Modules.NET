using FluentValidation;
using GrpcServer.Application.Common.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GrpcServer.Application
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TypeTestDtoValidator : AbstractValidator<TypeTestDto>
    {
        [IntentManaged(Mode.Merge)]
        public TypeTestDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.BinaryField)
                .NotNull();

            RuleFor(v => v.BinaryFieldCollection)
                .NotNull();

            RuleFor(v => v.BoolFieldCollection)
                .NotNull();

            RuleFor(v => v.ByteFieldCollection)
                .NotNull();

            RuleFor(v => v.CharFieldCollection)
                .NotNull();

            RuleFor(v => v.ComplexTypeField)
                .NotNull()
                .SetValidator(provider.GetValidator<ComplexTypeDto>()!);

            RuleFor(v => v.ComplexTypeFieldCollection)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<ComplexTypeDto>()!));

            RuleFor(v => v.ComplexTypeFieldNullable)
                .SetValidator(provider.GetValidator<ComplexTypeDto>()!);

            RuleFor(v => v.ComplexTypeFieldNullableCollection)
                .ForEach(x => x.SetValidator(provider.GetValidator<ComplexTypeDto>()!));

            RuleFor(v => v.DateOnlyFieldCollection)
                .NotNull();

            RuleFor(v => v.DateTimeFieldCollection)
                .NotNull();

            RuleFor(v => v.DateTimeOffsetFieldCollection)
                .NotNull();

            RuleFor(v => v.DecimalFieldCollection)
                .NotNull();

            RuleFor(v => v.DictionaryField)
                .NotNull();

            RuleFor(v => v.DictionaryFieldCollection)
                .NotNull();

            RuleFor(v => v.DoubleFieldCollection)
                .NotNull();

            RuleFor(v => v.FloatFieldCollection)
                .NotNull();

            RuleFor(v => v.GuidFieldCollection)
                .NotNull();

            RuleFor(v => v.IntFieldCollection)
                .NotNull();

            RuleFor(v => v.LongFieldCollection)
                .NotNull();

            RuleFor(v => v.ObjectField)
                .NotNull();

            RuleFor(v => v.ObjectFieldCollection)
                .NotNull();

            RuleFor(v => v.PagedResultField)
                .NotNull();

            RuleFor(v => v.ShortFieldCollection)
                .NotNull();

            RuleFor(v => v.StringField)
                .NotNull();

            RuleFor(v => v.StringFieldCollection)
                .NotNull();

            RuleFor(v => v.TimeSpanFieldCollection)
                .NotNull();
        }
    }
}