using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Supers.GetSupers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetSupersQueryValidator : AbstractValidator<GetSupersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetSupersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}