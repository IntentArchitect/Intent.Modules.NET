using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.FluentValidation.ValidationServiceInterface", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Common.Validation
{
    public interface IValidationService
    {
        Task Validate<TRequest>(TRequest request, CancellationToken cancellationToken = default);
    }
}