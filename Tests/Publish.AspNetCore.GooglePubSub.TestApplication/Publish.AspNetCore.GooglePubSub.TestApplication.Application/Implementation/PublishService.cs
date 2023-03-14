using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class PublishService : IPublishService
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PublishService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task TestPublish(string message)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}