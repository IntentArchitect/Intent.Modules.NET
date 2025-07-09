using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntitySingleParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SingleIndexEntitySingleParentCreateDtoValidator : AbstractValidator<SingleIndexEntitySingleParentCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public SingleIndexEntitySingleParentCreateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.SingleIndexEntitySingleChild)
                .NotNull()
                .SetValidator(provider.GetValidator<SingleIndexEntitySingleChildDto>()!);
        }
    }
}