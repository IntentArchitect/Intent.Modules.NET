using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeInts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdTypeIntsService : IDisposable
    {

        Task<int> Create(IdTypeIntCreateDto dto);

        Task<IdTypeIntDto> FindById(int id);

        Task<List<IdTypeIntDto>> FindAll();

        Task Put(int id, IdTypeIntUpdateDto dto);

        Task<IdTypeIntDto> Delete(int id);

    }
}