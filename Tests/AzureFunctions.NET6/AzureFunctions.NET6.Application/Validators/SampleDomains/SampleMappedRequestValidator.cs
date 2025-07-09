using System;
using AzureFunctions.NET6.Application.SampleDomains;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.SampleDomains
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SampleMappedRequestValidator : AbstractValidator<SampleMappedRequest>
    {
        [IntentManaged(Mode.Merge)]
        public SampleMappedRequestValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field)
                .NotNull();
        }
    }
}