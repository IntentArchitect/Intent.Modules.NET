using FluentValidation;
using GrpcServer.Application.Common.Validation;
using GrpcServer.Application.TypeTests;
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
                .NotNull()
                .SetValidator(provider.GetValidator<BinaryTestDto>()!);

            RuleFor(v => v.BoolField)
                .NotNull()
                .SetValidator(provider.GetValidator<BoolTestDto>()!);

            RuleFor(v => v.ByteField)
                .NotNull()
                .SetValidator(provider.GetValidator<ByteTestDto>()!);

            RuleFor(v => v.CharField)
                .NotNull()
                .SetValidator(provider.GetValidator<CharTestDto>()!);

            RuleFor(v => v.ComplexValueField)
                .NotNull()
                .SetValidator(provider.GetValidator<ComplexValueTestDto>()!);

            RuleFor(v => v.DateOnlyField)
                .NotNull()
                .SetValidator(provider.GetValidator<DateOnlyTestDto>()!);

            RuleFor(v => v.DateTimeField)
                .NotNull()
                .SetValidator(provider.GetValidator<DateTimeTestDto>()!);

            RuleFor(v => v.DateTimeOffsetField)
                .NotNull()
                .SetValidator(provider.GetValidator<DateTimeOffsetTestDto>()!);

            RuleFor(v => v.DecimalField)
                .NotNull()
                .SetValidator(provider.GetValidator<DecimalTestDto>()!);

            RuleFor(v => v.DictionaryField)
                .NotNull()
                .SetValidator(provider.GetValidator<DictionaryTestDto>()!);

            RuleFor(v => v.DoubleField)
                .NotNull()
                .SetValidator(provider.GetValidator<DoubleTestDto>()!);

            RuleFor(v => v.EnumField)
                .NotNull()
                .SetValidator(provider.GetValidator<EnumTestDto>()!);

            RuleFor(v => v.FloatField)
                .NotNull()
                .SetValidator(provider.GetValidator<FloatTestDto>()!);

            RuleFor(v => v.GuidField)
                .NotNull()
                .SetValidator(provider.GetValidator<GuidTestDto>()!);

            RuleFor(v => v.IntField)
                .NotNull()
                .SetValidator(provider.GetValidator<IntTestDto>()!);

            RuleFor(v => v.LongField)
                .NotNull()
                .SetValidator(provider.GetValidator<LongTestDto>()!);

            RuleFor(v => v.ObjectField)
                .NotNull()
                .SetValidator(provider.GetValidator<ObjectTestDto>()!);

            RuleFor(v => v.PagedResultField)
                .NotNull()
                .SetValidator(provider.GetValidator<PagedResultTestDto>()!);

            RuleFor(v => v.ShortField)
                .NotNull()
                .SetValidator(provider.GetValidator<ShortTestDto>()!);

            RuleFor(v => v.StringField)
                .NotNull()
                .SetValidator(provider.GetValidator<StringTestDto>()!);

            RuleFor(v => v.TimeSpanField)
                .NotNull()
                .SetValidator(provider.GetValidator<TimeSpanTestDto>()!);
        }
    }
}