using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeLongs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdTypeLongsService : IDisposable
    {

        Task<long> Create(IdTypeLongCreateDto dto);

        Task<IdTypeLongDto> FindById(long id);

        Task<List<IdTypeLongDto>> FindAll();

        Task Put(long id, IdTypeLongUpdateDto dto);

        Task<IdTypeLongDto> Delete(long id);

    }
}