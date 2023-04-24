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

        Task<string> CreateIdTypeOjectIdStr(IdTypeOjectIdStrCreateDto dto);

        Task<IdTypeOjectIdStrDto> FindIdTypeOjectIdStrById(string id);

        Task<List<IdTypeOjectIdStrDto>> FindIdTypeOjectIdStrs();

        Task UpdateIdTypeOjectIdStr(string id, IdTypeOjectIdStrUpdateDto dto);

        Task DeleteIdTypeOjectIdStr(string id);

    }
}