using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateRootDtoValidator : AbstractValidator<AggregateRootDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public AggregateRootDtoValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.AggregateAttr)
                .NotNull();

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<AggregateRootCompositeManyBDto>>()!));

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetRequiredService<IValidator<AggregateRootCompositeSingleADto>>()!);

            RuleFor(v => v.Aggregate)
                .SetValidator(provider.GetRequiredService<IValidator<AggregateRootAggregateSingleCDto>>()!);

            RuleFor(v => v.EnumType1)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.EnumType2)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.EnumType3)
                .NotNull()
                .IsInEnum();
        }
    }
}