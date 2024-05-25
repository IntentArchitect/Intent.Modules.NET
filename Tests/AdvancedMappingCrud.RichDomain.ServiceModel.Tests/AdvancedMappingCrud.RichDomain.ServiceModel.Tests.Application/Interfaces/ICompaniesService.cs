using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Companies;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces
{
    public interface ICompaniesService : IDisposable
    {
        Task<Guid> CreateCompany(CompanyCreateDto dto, CancellationToken cancellationToken = default);
        Task<CompanyDto> FindCompanyById(Guid id, CancellationToken cancellationToken = default);
        Task<List<CompanyDto>> FindCompanies(CancellationToken cancellationToken = default);
        Task DeleteCompany(Guid id, CancellationToken cancellationToken = default);
    }
}