using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class UploadDownloadService : IUploadDownloadService
    {
        private readonly IFileUploadRepository _fileUploadRepository;

        [IntentManaged(Mode.Merge)]
        public UploadDownloadService(IFileUploadRepository fileUploadRepository)
        {
            _fileUploadRepository = fileUploadRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Upload(
            Stream content,
            string? filename,
            string? contentType,
            long? contentLength,
            CancellationToken cancellationToken = default)
        {
            var entity = new FileUpload()
            {
                Filename = filename ?? "hello",
                ContentType = contentType ?? "application/octet-stream"
            };
            using (MemoryStream ms = new())
            {
                await content.CopyToAsync(ms);
                entity.Content = ms.ToArray();
            }
            _fileUploadRepository.Add(entity);
            await _fileUploadRepository.UnitOfWork.SaveChangesAsync();
            return entity.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<FileDownloadDto> Download(Guid id, CancellationToken cancellationToken = default)
        {
            var file = await _fileUploadRepository.FindByIdAsync(id, cancellationToken);
            if (file is null)
            {
                throw new NotFoundException($"Could not find FileUpload '{id}'");
            }
            return new FileDownloadDto() { Content = new MemoryStream(file.Content), ContentType = file.ContentType };
        }

        public void Dispose()
        {
        }
    }
}