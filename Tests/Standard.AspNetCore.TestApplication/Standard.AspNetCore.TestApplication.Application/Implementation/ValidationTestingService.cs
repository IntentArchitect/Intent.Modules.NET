using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Application.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ValidationTestingService : IValidationTestingService
    {
        [IntentManaged(Mode.Merge)]
        public ValidationTestingService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task InboundValidationDtoAction(
            InboundValidationDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement InboundValidationDtoAction (ValidationTestingService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}