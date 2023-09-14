using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ImplicitKeyAggrRootDtoValidator : AbstractValidator<ImplicitKeyAggrRootDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public ImplicitKeyAggrRootDtoValidator(IServiceProvider provider)
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
                .ForEach(x => x.SetValidator(provider.GetRequiredService<IValidator<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>>()!));
        }
    }
}