using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.Addresses;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressServiceCustomHandlers
{
    [IntentManaged(Mode.Merge)]
    public class CreateAddressNoTokenSCH
    {
        [IntentManaged(Mode.Merge)]
        public CreateAddressNoTokenSCH()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Guid> Handle(CreateAddressDto dto)
        {
            throw new NotImplementedException("Implement your business logic for this service call in the <#=ClassName#> (SCH = Service Call Handler) class.");
        }
    }
}