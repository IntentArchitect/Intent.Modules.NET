using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.UploadFile
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Guid>
    {
        private readonly IFileUploadRepository _fileUploadRepository;
        [IntentManaged(Mode.Merge)]
        public UploadFileCommandHandler(IFileUploadRepository fileUploadRepository)
        {
            _fileUploadRepository = fileUploadRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var entity = new FileUpload()
            {
                Filename = request.Filename,
                ContentType = request.ContentType
            };
            using (MemoryStream ms = new())
            {
                await request.Content.CopyToAsync(ms);
                entity.Content = ms.ToArray();
            }
            _fileUploadRepository.Add(entity);
            await _fileUploadRepository.UnitOfWork.SaveChangesAsync();
            return entity.Id;
        }
    }
}