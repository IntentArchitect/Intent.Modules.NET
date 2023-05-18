using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.TestValidation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestValidationQueryHandler : IRequestHandler<TestValidationQuery, int>
    {
        [IntentManaged(Mode.Ignore)]
        public TestValidationQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> Handle(TestValidationQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}