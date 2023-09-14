using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot
{
    public class CreateImplicitKeyAggrRootCommandValidator : AbstractValidator<CreateImplicitKeyAggrRootCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateImplicitKeyAggrRootCommandValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.ImplicitKeyNestedCompositions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto>>()!));
        }
    }
}