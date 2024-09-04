using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SecondService : ISecondService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public SecondService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoIt()
        {
            // TODO: Implement DoIt (SecondService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}