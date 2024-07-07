using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices;
using ProxyServiceTests.Proxy.PTH.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class DeleteAccountsService : Interfaces.IDeleteAccountsService
    {
        private readonly IntegrationServices.IDeleteAccountsService _ntegrationServicesIDeleteAccountsService;

        [IntentManaged(Mode.Merge)]
        public DeleteAccountsService(IntegrationServices.IDeleteAccountsService ntegrationServicesIDeleteAccountsService)
        {
            _ntegrationServicesIDeleteAccountsService = ntegrationServicesIDeleteAccountsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteAccountCommand(Guid id, CancellationToken cancellationToken = default)
        {
            await _ntegrationServicesIDeleteAccountsService.DeleteAccountAsync(id, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}