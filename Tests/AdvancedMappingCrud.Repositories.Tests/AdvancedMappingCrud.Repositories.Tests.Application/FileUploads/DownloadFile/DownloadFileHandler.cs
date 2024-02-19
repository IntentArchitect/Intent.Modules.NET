using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.DownloadFile
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DownloadFileHandler : IRequestHandler<DownloadFile, FileDownloadDto>
    {
        private readonly IFileUploadRepository _fileUploadRepository;

        [IntentManaged(Mode.Merge)]
        public DownloadFileHandler(IFileUploadRepository fileUploadRepository)
        {
            _fileUploadRepository = fileUploadRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<FileDownloadDto> Handle(DownloadFile request, CancellationToken cancellationToken)
        {
            var file = await _fileUploadRepository.FindByIdAsync(request.Id, cancellationToken);
            if (file is null)
            {
                throw new NotFoundException($"Could not find FileUpload '{request.Id}'");
            }

            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}