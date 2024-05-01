using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Domain.Entities;
using OutputCachingRedis.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OutputCachingRedis.Tests.Application.Files.CreateFiles
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateFilesCommandHandler : IRequestHandler<CreateFilesCommand, Guid>
    {
        private readonly IFilesRepository _filesRepository;

        [IntentManaged(Mode.Merge)]
        public CreateFilesCommandHandler(IFilesRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateFilesCommand request, CancellationToken cancellationToken)
        {
            var files = new Domain.Entities.Files
            {
                ContentType = request.ContentType
            };

            using (MemoryStream ms = new())
            {
                await request.Content.CopyToAsync(ms);
                files.Content = ms.ToArray();
            }

            _filesRepository.Add(files);
            await _filesRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return files.Id;
        }
    }
}