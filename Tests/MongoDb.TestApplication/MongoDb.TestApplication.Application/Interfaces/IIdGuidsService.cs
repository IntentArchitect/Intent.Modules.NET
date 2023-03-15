using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdGuids;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdGuidsService : IDisposable
    {

        Task<Guid> Create(IdGuidCreateDto dto);

        Task<IdGuidDto> FindById(Guid id);

        Task<List<IdGuidDto>> FindAll();

        Task Put(Guid id, IdGuidUpdateDto dto);

        Task<IdGuidDto> Delete(Guid id);

    }
}