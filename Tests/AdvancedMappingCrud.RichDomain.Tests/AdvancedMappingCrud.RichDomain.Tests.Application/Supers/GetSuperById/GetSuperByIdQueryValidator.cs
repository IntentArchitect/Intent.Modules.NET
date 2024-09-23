using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Supers.GetSuperById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetSuperByIdQueryValidator : AbstractValidator<GetSuperByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetSuperByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}