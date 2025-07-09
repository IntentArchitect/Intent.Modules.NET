using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompoundIndexEntitySingleParentCreateDtoValidator : AbstractValidator<CompoundIndexEntitySingleParentCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public CompoundIndexEntitySingleParentCreateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.CompoundIndexEntitySingleChild)
                .NotNull()
                .SetValidator(provider.GetValidator<CompoundIndexEntitySingleChildDto>()!);
        }
    }
}