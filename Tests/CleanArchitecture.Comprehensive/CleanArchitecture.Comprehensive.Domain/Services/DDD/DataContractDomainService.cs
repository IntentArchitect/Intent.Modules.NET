using System;
using CleanArchitecture.Comprehensive.Domain.Contracts.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Services.DDD
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
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}