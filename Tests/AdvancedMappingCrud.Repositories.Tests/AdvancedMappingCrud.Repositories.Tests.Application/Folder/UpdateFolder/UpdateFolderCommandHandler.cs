using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.Folder;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Folder.UpdateFolder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateFolderCommandHandler : IRequestHandler<UpdateFolderCommand>
    {
        private readonly IFolderRepository _folderRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateFolderCommandHandler(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateFolderCommand request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (folder is null)
            {
                throw new NotFoundException($"Could not find Folder '{request.Id}'");
            }

            folder.Name = request.Name;
            folder.Code = request.Code;
        }
    }
}