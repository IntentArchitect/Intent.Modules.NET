using System;
using AzureFunctions.TestApplication.Application.CosmosDB;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Validators.CosmosDB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CosmosChangeDtoValidator : AbstractValidator<CosmosChangeDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public CosmosChangeDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}