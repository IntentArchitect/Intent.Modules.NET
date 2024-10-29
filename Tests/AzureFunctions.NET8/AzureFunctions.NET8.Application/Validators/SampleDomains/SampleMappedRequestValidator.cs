using System;
using AzureFunctions.NET8.Application.SampleDomains;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.SampleDomains
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SampleMappedRequestValidator : AbstractValidator<SampleMappedRequest>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public SampleMappedRequestValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field)
                .NotNull();
        }
    }
}