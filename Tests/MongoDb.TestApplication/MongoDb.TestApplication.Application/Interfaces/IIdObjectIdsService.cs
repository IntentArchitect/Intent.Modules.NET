using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdObjectIds;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdObjectIdsService : IDisposable
    {

        Task<string> Create(IdObjectIdCreateDto dto);

        Task<IdObjectIdDto> FindById(string id);

        Task<List<IdObjectIdDto>> FindAll();

        Task Put(string id, IdObjectIdUpdateDto dto);

        Task<IdObjectIdDto> Delete(string id);

    }
}