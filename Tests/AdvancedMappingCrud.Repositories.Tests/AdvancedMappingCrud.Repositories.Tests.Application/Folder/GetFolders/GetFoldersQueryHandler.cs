using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.Folder;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Folder.GetFolders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetFoldersQueryHandler : IRequestHandler<GetFoldersQuery, List<FolderDto>>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetFoldersQueryHandler(IFolderRepository folderRepository, IMapper mapper)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<FolderDto>> Handle(GetFoldersQuery request, CancellationToken cancellationToken)
        {
            var folders = await _folderRepository.FindAllAsync(cancellationToken);
            return folders.MapToFolderDtoList(_mapper);
        }
    }
}