using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace IdentityServer4CoHosted.Contracts
{

    public interface ITestService : IDisposable
    {

        Task AnonymousOperation();

        Task AuthenticatedOperation();

        Task AuthorizedOperation();

    }
}