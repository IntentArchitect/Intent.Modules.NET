using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.MyOpDomainServiceTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MyOpDomainServiceTestCommandValidator : AbstractValidator<MyOpDomainServiceTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public MyOpDomainServiceTestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}