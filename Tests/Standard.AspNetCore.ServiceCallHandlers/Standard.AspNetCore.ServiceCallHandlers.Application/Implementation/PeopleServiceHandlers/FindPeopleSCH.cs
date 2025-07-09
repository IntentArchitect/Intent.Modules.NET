using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Application.People;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers
{
    [IntentManaged(Mode.Merge)]
    public class FindPeopleSCH
    {
        [IntentManaged(Mode.Merge)]
        public FindPeopleSCH()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<PersonDto>> Handle(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Implement your business logic for this service call in the <#=ClassName#> (SCH = Service Call Handler) class.");
        }
    }
}