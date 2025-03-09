using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Application.Tags;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GrpcServer.Application.Interfaces
{
    public interface ITagsService
    {
        Task<Guid> CreateTag(TagCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateTag(Guid id, TagUpdateDto dto, CancellationToken cancellationToken = default);
        Task<TagDto> FindTagById(Guid id, CancellationToken cancellationToken = default);
        Task<List<TagDto>> FindTags(CancellationToken cancellationToken = default);
        Task DeleteTag(Guid id, CancellationToken cancellationToken = default);
    }
}