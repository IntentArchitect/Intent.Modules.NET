using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MultikeyIndexEntitySingleParentCreateDtoValidator : AbstractValidator<MultikeyIndexEntitySingleParentCreateDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public MultikeyIndexEntitySingleParentCreateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.MultikeyIndexEntitySingleChild)
                .NotNull()
                .SetValidator(provider.GetValidator<MultikeyIndexEntitySingleChildDto>()!);
        }
    }
}