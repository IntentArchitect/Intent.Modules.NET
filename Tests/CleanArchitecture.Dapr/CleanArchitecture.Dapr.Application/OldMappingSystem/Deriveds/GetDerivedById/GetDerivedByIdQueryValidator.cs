using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.GetDerivedById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDerivedByIdQueryValidator : AbstractValidator<GetDerivedByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetDerivedByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}