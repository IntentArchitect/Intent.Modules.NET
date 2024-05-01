using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common;
using OutputCachingRedis.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Files.GetFilesById
{
    public class GetFilesByIdQuery : IRequest<FileDownloadDto>, IQuery
    {
        public GetFilesByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}