using System;
using AzureFunctions.NET8.Application.SampleDomains;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.SampleDomains
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SampleDomainCreateDtoValidator : AbstractValidator<SampleDomainCreateDto>
    {
        public SampleDomainCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}