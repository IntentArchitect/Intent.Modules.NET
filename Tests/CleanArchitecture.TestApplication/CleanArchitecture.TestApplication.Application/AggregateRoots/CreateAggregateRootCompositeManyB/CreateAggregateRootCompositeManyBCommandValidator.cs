using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB
{
    public class CreateAggregateRootCompositeManyBCommandValidator : AbstractValidator<CreateAggregateRootCompositeManyBCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateAggregateRootCompositeManyBCommandValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.CompositeAttr)
                .NotNull();

            RuleFor(v => v.Composite)
                .SetValidator(provider.GetRequiredService<IValidator<CreateAggregateRootCompositeManyBCompositeSingleBBDto>>()!);

            RuleFor(v => v.Composites)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<CreateAggregateRootCompositeManyBCompositeManyBBDto>>()!));
        }
    }
}