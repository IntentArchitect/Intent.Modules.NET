using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.ClassAS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{

    public interface IClassASService : IDisposable
    {
        Task Create(ClassACreateDTO dto, CancellationToken cancellationToken = default);
        Task<ClassADTO> FindById(Guid id, CancellationToken cancellationToken = default);
        Task<List<ClassADTO>> FindAll(CancellationToken cancellationToken = default);
        Task Update(Guid id, ClassAUpdateDTO dto, CancellationToken cancellationToken = default);
        Task Delete(Guid id, CancellationToken cancellationToken = default);

    }
}