using System;
using FastEndpointsTest.Domain.Contracts.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace FastEndpointsTest.Domain.Services.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DataContractDomainService : IDataContractDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public DataContractDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DataContractObject Operation()
        {
            // TODO: Implement Operation (DataContractDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}