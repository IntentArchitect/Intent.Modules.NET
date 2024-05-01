using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common;
using OutputCachingRedis.Tests.Domain.Common.Exceptions;
using OutputCachingRedis.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Files.GetFilesById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetFilesByIdQueryHandler : IRequestHandler<GetFilesByIdQuery, FileDownloadDto>
    {
        private readonly IFilesRepository _filesRepository;

        [IntentManaged(Mode.Merge)]
        public GetFilesByIdQueryHandler(IFilesRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<FileDownloadDto> Handle(GetFilesByIdQuery request, CancellationToken cancellationToken)
        {
            var files = await _filesRepository.FindByIdAsync(request.Id, cancellationToken);
            if (files is null)
            {
                throw new NotFoundException($"Could not find Files '{request.Id}'");
            }

            return new FileDownloadDto() { Content = new MemoryStream(files.Content), ContentType = files.ContentType };
        }
    }
}