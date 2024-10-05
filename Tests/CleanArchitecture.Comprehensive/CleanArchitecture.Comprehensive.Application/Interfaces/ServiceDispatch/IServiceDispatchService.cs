using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Interfaces.ServiceDispatch
{
    public interface IServiceDispatchService : IDisposable
    {
        void Mutation1();
        void Mutation2(string param);
        Task Mutation3Async(CancellationToken cancellationToken = default);
        Task Mutation4Async(string param, CancellationToken cancellationToken = default);
        string Query5(string param);
        string Query6();
        Task<string> Query7Async(CancellationToken cancellationToken = default);
        Task<string> Query8Async(string param, CancellationToken cancellationToken = default);
    }
}