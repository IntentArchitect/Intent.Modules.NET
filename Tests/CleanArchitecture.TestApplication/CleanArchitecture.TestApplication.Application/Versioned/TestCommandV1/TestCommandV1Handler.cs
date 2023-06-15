using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Versioned.TestCommandV1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestCommandV1Handler : IRequestHandler<TestCommandV1>
    {
        public const string ExpectedInput = "123";
        
        [IntentManaged(Mode.Merge)]
        public TestCommandV1Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Unit> Handle(TestCommandV1 request, CancellationToken cancellationToken)
        {
            Assert.Equal(ExpectedInput, request.Value);
            return Unit.Value;
        }
    }
}