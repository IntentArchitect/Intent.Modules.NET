using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRootLongs.UpdateAggregateRootLong
{
    public class UpdateAggregateRootLongCommandValidator : AbstractValidator<UpdateAggregateRootLongCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public UpdateAggregateRootLongCommandValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.CompositeOfAggrLong)
                .SetValidator(provider.GetRequiredService<IValidator<UpdateAggregateRootLongCompositeOfAggrLongDto>>()!);
        }
    }
}