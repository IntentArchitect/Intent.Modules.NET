using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeOjectIdStrs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdTypeOjectIdStrsService : IDisposable
    {

        Task<string> Create(IdTypeOjectIdStrCreateDto dto);

        Task<IdTypeOjectIdStrDto> FindById(string id);

        Task<List<IdTypeOjectIdStrDto>> FindAll();

        Task Put(string id, IdTypeOjectIdStrUpdateDto dto);

        Task<IdTypeOjectIdStrDto> Delete(string id);

    }
}