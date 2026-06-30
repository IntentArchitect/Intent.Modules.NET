using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Folder;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.Folder;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Folder.CreateFolder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, Guid>
    {
        private readonly IFolderRepository _folderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateFolderCommandHandler(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            var folder = new Domain.Entities.Folder.Folder(
                name: request.Name,
                code: request.Code);

            _folderRepository.Add(folder);
            await _folderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return folder.Id;
        }
    }
}