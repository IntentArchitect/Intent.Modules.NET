using System;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.UpdateAggregateRootLong
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAggregateRootLongCommandValidator : AbstractValidator<UpdateAggregateRootLongCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAggregateRootLongCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.CompositeOfAggrLong)
                .SetValidator(provider.GetValidator<UpdateAggregateRootLongCompositeOfAggrLongDto>()!);
        }
    }
}