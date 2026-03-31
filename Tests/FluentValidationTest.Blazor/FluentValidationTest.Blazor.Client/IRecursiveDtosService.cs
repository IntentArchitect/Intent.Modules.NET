using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidationTest.Blazor.Client.Contracts.Services.ValidationScenarios.RecursiveDtos;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace FluentValidationTest.Blazor.Client
{
    public interface IRecursiveDtosService : IDisposable
    {
        Task ValidateRecursiveNodeAsync(RecursiveNodeDto root, CancellationToken cancellationToken = default);
    }
}