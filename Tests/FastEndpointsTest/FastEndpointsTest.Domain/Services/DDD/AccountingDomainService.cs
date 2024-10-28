using System;
using FastEndpointsTest.Domain.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace FastEndpointsTest.Domain.Services.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AccountingDomainService : IAccountingDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public AccountingDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void Transfer(string fromAccNumber, string toAccNumber, Money amount, string description)
        {
            // TODO: Implement Transfer (AccountingDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}