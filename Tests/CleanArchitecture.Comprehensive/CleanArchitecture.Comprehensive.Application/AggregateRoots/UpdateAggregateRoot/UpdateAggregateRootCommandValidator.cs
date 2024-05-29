using System;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.UpdateAggregateRoot
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateRootCommandValidator : AbstractValidator<UpdateAggregateRootCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.AggregateAttr)
                .NotNull();

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateAggregateRootCompositeManyBDto>()!));

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetValidator<UpdateAggregateRootCompositeSingleADto>()!);

            RuleFor(v => v.Aggregate)
                .SetValidator(provider.GetValidator<UpdateAggregateRootAggregateSingleCDto>()!);

            RuleFor(v => v.LimitedDomain)
                .NotNull()
                .MaximumLength(10);

            RuleFor(v => v.LimitedService)
                .NotNull()
                .MaximumLength(20);
        }
    }
}