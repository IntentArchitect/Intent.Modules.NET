using System;
using Ardalis.Domain.Entities;
using Ardalis.Specification;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Ardalis.Repositories.Specification", Version = "1.0")]

namespace Ardalis.Domain.Specifications
{
    [IntentManaged(Mode.Merge)]
    public class ClientSpec : Specification<Client>
    {
        public ClientSpec()
        {
        }

        public ClientSpec(Guid id)
        {
            Query.Where(x => x.Id == id);
        }
    }
}