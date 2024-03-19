using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.GetDomainServiceTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDomainServiceTestsQueryValidator : AbstractValidator<GetDomainServiceTestsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDomainServiceTestsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}