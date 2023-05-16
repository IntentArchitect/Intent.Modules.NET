using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application.Interfaces
{
    public interface ITagService : IDisposable
    {
        Task Create(CreateTagDto dto);
        Task<List<TagDto>> GetAll();
    }
}