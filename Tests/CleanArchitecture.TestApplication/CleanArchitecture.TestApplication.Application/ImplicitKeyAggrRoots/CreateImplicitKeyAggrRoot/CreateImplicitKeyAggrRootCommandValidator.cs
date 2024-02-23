using System;
using CleanArchitecture.TestApplication.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateImplicitKeyAggrRootCommandValidator : AbstractValidator<CreateImplicitKeyAggrRootCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateImplicitKeyAggrRootCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.ImplicitKeyNestedCompositions)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto>()!));
        }
    }
}