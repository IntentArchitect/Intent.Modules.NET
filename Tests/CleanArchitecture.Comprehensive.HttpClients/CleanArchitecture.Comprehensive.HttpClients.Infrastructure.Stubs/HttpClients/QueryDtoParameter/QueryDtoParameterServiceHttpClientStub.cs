using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.QueryDtoParameter;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.Stubs.HttpClientStub", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure.Stubs.HttpClients.QueryDtoParameter
{
    public class QueryDtoParameterServiceHttpClientStub : IQueryDtoParameterService
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> HasDtoParameterAsync(
            QueryDtoParameterCriteria arg,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(0);
        }

        public void Dispose()
        {
        }
    }
}