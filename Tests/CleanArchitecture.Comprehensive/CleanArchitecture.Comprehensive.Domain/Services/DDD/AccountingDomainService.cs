using System;
using CleanArchitecture.Comprehensive.Domain.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Services.DDD
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
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}