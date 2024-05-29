using System;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.QueryDtoParameter.HasDtoParameter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HasDtoParameterQueryValidator : AbstractValidator<HasDtoParameterQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public HasDtoParameterQueryValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Arg)
                .NotNull()
                .SetValidator(provider.GetValidator<QueryDtoParameterCriteria>()!);
        }
    }
}