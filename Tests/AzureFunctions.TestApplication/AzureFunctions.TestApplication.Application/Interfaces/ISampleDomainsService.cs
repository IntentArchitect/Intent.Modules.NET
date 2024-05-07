using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Pagination;
using AzureFunctions.TestApplication.Application.SampleDomains;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Interfaces
{
    public interface ISampleDomainsService : IDisposable
    {
        Task<Guid> CreateSampleDomain(SampleDomainCreateDto dto, CancellationToken cancellationToken = default);
        Task<SampleDomainDto> FindSampleDomainById(Guid id, CancellationToken cancellationToken = default);
        Task<List<SampleDomainDto>> FindSampleDomains(CancellationToken cancellationToken = default);
        Task UpdateSampleDomain(Guid id, SampleDomainUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteSampleDomain(Guid id, CancellationToken cancellationToken = default);
        Task<string> MappedAzureFunction(SampleMappedRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<SampleDomainDto>> FindSampleDomainsPaged(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// This should print out a warning to say that the query should also be set to a collection since the service operation expects to return a collection
        /// </summary>
        Task<List<SampleDomainDto>> FindByNameForSingleSampleDomainMapToCollection(string name, CancellationToken cancellationToken = default);
        Task<List<SampleDomainDto>> FindSampleDomainsByName(string name, CancellationToken cancellationToken = default);
        /// <summary>
        /// This is not meant to be implemented automatically
        /// </summary>
        /// <param name="attribute">Comment for this parameter</param>
        Task<SampleDomainDto> FindSampleDomainByAttribute(string attribute, CancellationToken cancellationToken = default);
    }
}