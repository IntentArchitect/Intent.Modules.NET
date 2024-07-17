using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace HashiCorpVault.Application.VaultTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class VaultTestCommandHandler : IRequestHandler<VaultTestCommand>
    {
        private readonly IConfiguration _configuration;

        [IntentManaged(Mode.Merge)]
        public VaultTestCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(VaultTestCommand request, CancellationToken cancellationToken)
        {
            var test = _configuration["passcode"];
        }
    }
}