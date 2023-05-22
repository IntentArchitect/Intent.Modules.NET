using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.IdTypeOjectIdStrs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Interfaces
{

    public interface IIdTypeOjectIdStrsService : IDisposable
    {
        Task<string> CreateIdTypeOjectIdStr(IdTypeOjectIdStrCreateDto dto, CancellationToken cancellationToken = default);
        Task<IdTypeOjectIdStrDto> FindIdTypeOjectIdStrById(string id, CancellationToken cancellationToken = default);
        Task<List<IdTypeOjectIdStrDto>> FindIdTypeOjectIdStrs(CancellationToken cancellationToken = default);
        Task UpdateIdTypeOjectIdStr(string id, IdTypeOjectIdStrUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteIdTypeOjectIdStr(string id, CancellationToken cancellationToken = default);

    }
}