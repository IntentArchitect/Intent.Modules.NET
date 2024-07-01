using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.BugFixes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices
{
    public interface IBugFixesService : IDisposable
    {
        Task<TaskNameDto> GetTaskNameAsync(CancellationToken cancellationToken = default);
    }
}