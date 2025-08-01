using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Application.Interfaces.ServiceDispatch;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FastEndpointsTest.Application.Implementation.ServiceDispatch
{
    [IntentManaged(Mode.Merge)]
    public class ServiceDispatchService : IServiceDispatchService
    {
        [IntentManaged(Mode.Merge)]
        public ServiceDispatchService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Mutation1()
        {
            // TODO: Implement Mutation1 (ServiceDispatchService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Mutation2(string param)
        {
            // TODO: Implement Mutation2 (ServiceDispatchService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Mutation3Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Mutation3Async (ServiceDispatchService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Mutation4Async(string param, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Mutation4Async (ServiceDispatchService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public string Query5(string param)
        {
            // TODO: Implement Query5 (ServiceDispatchService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public string Query6()
        {
            // TODO: Implement Query6 (ServiceDispatchService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> Query7Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Query7Async (ServiceDispatchService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> Query8Async(string param, CancellationToken cancellationToken = default)
        {
            // TODO: Implement Query8Async (ServiceDispatchService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}