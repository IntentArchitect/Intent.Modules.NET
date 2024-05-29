using System;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.CreateAggregateRootLong
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAggregateRootLongCommandValidator : AbstractValidator<CreateAggregateRootLongCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAggregateRootLongCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.CompositeOfAggrLong)
                .SetValidator(provider.GetValidator<CreateAggregateRootLongCompositeOfAggrLongDto>()!);
        }
    }
}