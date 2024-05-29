using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Unversioned.Test
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        public const string ExpectedInput = "789";

        [IntentManaged(Mode.Merge)]
        public TestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestCommand request, CancellationToken cancellationToken)
        {
            Assert.Equal(ExpectedInput, request.Value);

        }
    }
}