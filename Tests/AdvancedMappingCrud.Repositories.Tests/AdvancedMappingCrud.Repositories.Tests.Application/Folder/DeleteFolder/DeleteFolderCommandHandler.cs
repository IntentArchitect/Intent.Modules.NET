using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.Folder;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Folder.DeleteFolder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteFolderCommandHandler : IRequestHandler<DeleteFolderCommand>
    {
        private readonly IFolderRepository _folderRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteFolderCommandHandler(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (folder is null)
            {
                throw new NotFoundException($"Could not find Folder '{request.Id}'");
            }


            _folderRepository.Remove(folder);
        }
    }
}