using System;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransitFinbuckle.Test.Application.RequestResponse.Test
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        private readonly ILogger<TestCommandHandler> _logger;
        private readonly ITenantInfo _tenantInfo;

        [IntentManaged(Mode.Merge)]
        public TestCommandHandler(ILogger<TestCommandHandler> logger, ITenantInfo tenantInfo)
        {
            _logger = logger;
            _tenantInfo = tenantInfo;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Current Tenant: {Tenant}", _tenantInfo.Identifier);
        }
    }
}