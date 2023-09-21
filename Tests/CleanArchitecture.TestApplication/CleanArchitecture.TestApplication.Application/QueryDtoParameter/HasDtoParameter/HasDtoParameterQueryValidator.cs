using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.QueryDtoParameter.HasDtoParameter
{
    public class HasDtoParameterQueryValidator : AbstractValidator<HasDtoParameterQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public HasDtoParameterQueryValidator(IServiceProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IServiceProvider provider)
        {
            RuleFor(v => v.Arg)
                .NotNull()
                .SetValidator(provider.GetRequiredService<IValidator<QueryDtoParameterCriteria>>()!);
        }
    }
}