using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.GetDomainServiceTestById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDomainServiceTestByIdQueryValidator : AbstractValidator<GetDomainServiceTestByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDomainServiceTestByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}