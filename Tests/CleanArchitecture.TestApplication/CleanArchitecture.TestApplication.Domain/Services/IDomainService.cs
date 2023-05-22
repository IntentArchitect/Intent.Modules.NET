using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Services
{
    public interface IDomainService
    {
        void Mutation();
        string Query();
        void Mutation(string param);
        Task MutationAsync(CancellationToken cancellationToken = default);
        Task MutationAsync(string param, CancellationToken cancellationToken = default);
        string Query(string param);
        Task<string> QueryAsync(CancellationToken cancellationToken = default);
        Task<string> QueryAsync(string param, CancellationToken cancellationToken = default);
    }
}