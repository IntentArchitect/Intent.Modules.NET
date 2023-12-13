using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Customers;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation.Customers
{
    [IntentManaged(Mode.Merge)]
    public class PersonService : IPersonService
    {
        [IntentManaged(Mode.Merge)]
        public PersonService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<PersonDto> GetPersonById(Guid personId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}