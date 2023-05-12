using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.ClassAS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Interfaces
{

    public interface IClassASService : IDisposable
    {
        Task Create(ClassACreateDTO dto);
        Task<ClassADTO> FindById(Guid id);
        Task<List<ClassADTO>> FindAll();
        Task Update(Guid id, ClassAUpdateDTO dto);
        Task Delete(Guid id);

    }
}