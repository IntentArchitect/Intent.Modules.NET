using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Application.Interfaces
{

    public interface IPublishService : IDisposable
    {
        Task TestPublish(string message, CancellationToken cancellationToken = default);

    }
}