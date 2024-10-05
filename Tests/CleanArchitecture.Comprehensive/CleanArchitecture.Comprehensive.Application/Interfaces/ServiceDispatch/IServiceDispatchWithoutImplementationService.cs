using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Interfaces.ServiceDispatch
{
    public interface IServiceDispatchWithoutImplementationService : IDisposable
    {
        void MutationNoImpl1(string param);
        Task MutationNoImpl2Async(CancellationToken cancellationToken = default);
        Task MutationNoImpl3Async(string param, CancellationToken cancellationToken = default);
        string QueryNoImpl4(string param);
        string QueryNoImpl5();
        Task<string> QueryNoImpl6Async(CancellationToken cancellationToken = default);
        Task<string> QueryNoImpl7Async(string param, CancellationToken cancellationToken = default);
        void MutationNoImpl8();
    }
}