using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.TestApplication.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntityMultiParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SingleIndexEntityMultiParentCreateDtoValidator : AbstractValidator<SingleIndexEntityMultiParentCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public SingleIndexEntityMultiParentCreateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.SomeField)
                .NotNull();

            RuleFor(v => v.SingleIndexEntityMultiChild)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<SingleIndexEntityMultiChildDto>()!));
        }
    }
}