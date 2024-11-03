using System;
using AzureFunctions.NET8.Application.CosmosDB;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.CosmosDB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CosmosChangeDtoValidator : AbstractValidator<CosmosChangeDto>
    {
        public CosmosChangeDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}