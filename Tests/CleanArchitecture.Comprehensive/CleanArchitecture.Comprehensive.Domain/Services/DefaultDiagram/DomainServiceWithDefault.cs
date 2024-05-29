using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Services.DefaultDiagram
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DomainServiceWithDefault : IDomainServiceWithDefault
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public DomainServiceWithDefault()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void OperationWithDefault(string param1 = "Operation Param 1 Value")
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}