using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeGuids;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdTypeGuidsService : IDisposable
    {

        Task<Guid> CreateIdTypeGuid(IdTypeGuidCreateDto dto);

        Task<IdTypeGuidDto> FindIdTypeGuidById(Guid id);

        Task<List<IdTypeGuidDto>> FindIdTypeGuids();

        Task UpdateIdTypeGuid(Guid id, IdTypeGuidUpdateDto dto);

        Task DeleteIdTypeGuid(Guid id);

    }
}