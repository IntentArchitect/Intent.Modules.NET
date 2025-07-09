using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.GetCameraByIdemiaId
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCameraByIdemiaIdValidator : AbstractValidator<GetCameraByIdemiaId>
    {
        [IntentManaged(Mode.Merge)]
        public GetCameraByIdemiaIdValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.IdemiaId)
                .NotNull();
        }
    }
}