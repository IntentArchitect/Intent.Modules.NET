using System;
using AzureFunctions.NET6.Application.CosmosDB;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.CosmosDB
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CosmosChangeDtoValidator : AbstractValidator<CosmosChangeDto>
    {
        [IntentManaged(Mode.Merge)]
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