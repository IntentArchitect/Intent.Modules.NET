using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.SampleDomains;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Interfaces
{

    public interface ISampleDomainsService : IDisposable
    {

        Task<Guid> Create(SampleDomainCreateDTO dto);

        Task<SampleDomainDTO> FindById(Guid id);

        Task<List<SampleDomainDTO>> FindAll();

        Task Update(Guid id, SampleDomainUpdateDTO dto);

        Task Delete(Guid id);

    }
}