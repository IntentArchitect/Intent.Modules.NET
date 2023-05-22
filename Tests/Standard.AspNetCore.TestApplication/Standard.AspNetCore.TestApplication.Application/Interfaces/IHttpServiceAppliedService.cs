using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{

    public interface IHttpServiceAppliedService : IDisposable
    {
        Task<string> GetValue(CancellationToken cancellationToken = default);
        Task PostValue(string value, CancellationToken cancellationToken = default);
        Task NonAppliedOperation(CancellationToken cancellationToken = default);

    }
}