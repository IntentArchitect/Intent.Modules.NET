using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ListedUnlistedServicesService : IListedUnlistedServicesService
    {

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ListedUnlistedServicesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task ListedServiceFunc(string param)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UnlistedServiceFunc()
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}