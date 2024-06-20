using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CloudBlobStorageClients.Application.Tests.TestAzure
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestAzureCommandValidator : AbstractValidator<TestAzureCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestAzureCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}