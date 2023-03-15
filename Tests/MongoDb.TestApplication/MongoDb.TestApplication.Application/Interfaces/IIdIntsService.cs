using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdInts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdIntsService : IDisposable
    {

        Task<int> Create(IdIntCreateDto dto);

        Task<IdIntDto> FindById(int id);

        Task<List<IdIntDto>> FindAll();

        Task Put(int id, IdIntUpdateDto dto);

        Task<IdIntDto> Delete(int id);

    }
}