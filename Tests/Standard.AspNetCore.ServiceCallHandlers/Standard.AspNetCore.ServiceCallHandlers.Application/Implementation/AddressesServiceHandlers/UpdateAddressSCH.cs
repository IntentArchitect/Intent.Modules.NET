using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressesServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class UpdateAddressSCH
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAddressSCH()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Handle(Guid id)
        {
            throw new NotImplementedException("Implement your business logic for this service call in the <#=ClassName#> (SCH = Service Call Handler) class.");
        }
    }
}