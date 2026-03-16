using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressesServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class CreateAddressSCH
    {
        [IntentManaged(Mode.Merge)]
        public CreateAddressSCH()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Guid> Handle()
        {
            throw new NotImplementedException("Implement your business logic for this service call in the <#=ClassName#> (SCH = Service Call Handler) class.");
        }
    }
}