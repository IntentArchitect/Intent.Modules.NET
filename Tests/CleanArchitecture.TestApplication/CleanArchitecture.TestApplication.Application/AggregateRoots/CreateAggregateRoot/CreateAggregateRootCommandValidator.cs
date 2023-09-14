using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot
{
    public class CreateAggregateRootCommandValidator : AbstractValidator<CreateAggregateRootCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateAggregateRootCommandValidator(IServiceProvider provider)
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
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<CreateAggregateRootCompositeManyBDto>>()!));

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetRequiredService<IValidator<CreateAggregateRootCompositeSingleADto>>()!);

            RuleFor(v => v.Aggregate)
                .SetValidator(provider.GetRequiredService<IValidator<CreateAggregateRootAggregateSingleCDto>>()!);

            RuleFor(v => v.LimitedDomain)
                .NotNull()
                .MaximumLength(10);

            RuleFor(v => v.LimitedService)
                .NotNull()
                .MaximumLength(20);
        }
    }
}