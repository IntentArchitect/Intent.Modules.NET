using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot
{
    public class UpdateAggregateRootCommandValidator : AbstractValidator<UpdateAggregateRootCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public UpdateAggregateRootCommandValidator(IServiceProvider provider)
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
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<UpdateAggregateRootCompositeManyBDto>>()!));

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetRequiredService<IValidator<UpdateAggregateRootCompositeSingleADto>>()!);

            RuleFor(v => v.Aggregate)
                .SetValidator(provider.GetRequiredService<IValidator<UpdateAggregateRootAggregateSingleCDto>>()!);

            RuleFor(v => v.LimitedDomain)
                .NotNull()
                .MaximumLength(10);

            RuleFor(v => v.LimitedService)
                .NotNull()
                .MaximumLength(20);
        }
    }
}