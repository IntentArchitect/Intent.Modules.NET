using System;
using AzureFunctions.NET6.Application.CosmosDB.CosmosChange;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.CosmosDB.CosmosChange
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CosmosChangeCommandValidator : AbstractValidator<CosmosChangeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CosmosChangeCommandValidator()
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