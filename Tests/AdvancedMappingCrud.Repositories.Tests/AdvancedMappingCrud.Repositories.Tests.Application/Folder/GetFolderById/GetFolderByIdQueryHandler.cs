using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.Folder;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Folder.GetFolderById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetFolderByIdQueryHandler : IRequestHandler<GetFolderByIdQuery, FolderDto>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetFolderByIdQueryHandler(IFolderRepository folderRepository, IMapper mapper)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<FolderDto> Handle(GetFolderByIdQuery request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (folder is null)
            {
                throw new NotFoundException($"Could not find Folder '{request.Id}'");
            }
            return folder.MapToFolderDto(_mapper);
        }
    }
}