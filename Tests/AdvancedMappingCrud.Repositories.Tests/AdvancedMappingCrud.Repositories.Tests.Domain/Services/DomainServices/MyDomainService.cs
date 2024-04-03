using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Services.DomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyDomainService : IMyDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public MyDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoSomething()
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}