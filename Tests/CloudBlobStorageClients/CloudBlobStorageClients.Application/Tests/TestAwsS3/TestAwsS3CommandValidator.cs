using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CloudBlobStorageClients.Application.Tests.TestAwsS3
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestAwsS3CommandValidator : AbstractValidator<TestAwsS3Command>
    {
        [IntentManaged(Mode.Merge)]
        public TestAwsS3CommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}