using System;
using AzureFunctions.NET8.Application.SampleDomains;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.SampleDomains
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SampleMappedRequestValidator : AbstractValidator<SampleMappedRequest>
    {
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