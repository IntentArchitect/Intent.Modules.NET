using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdLongs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdLongsService : IDisposable
    {

        Task<long> Create(IdLongCreateDto dto);

        Task<IdLongDto> FindById(long id);

        Task<List<IdLongDto>> FindAll();

        Task Put(long id, IdLongUpdateDto dto);

        Task<IdLongDto> Delete(long id);

    }
}