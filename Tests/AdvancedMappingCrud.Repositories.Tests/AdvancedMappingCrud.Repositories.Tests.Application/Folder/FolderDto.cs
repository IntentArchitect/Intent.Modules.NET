using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Folder;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Folder
{
    public class FolderDto : IMapFrom<Domain.Entities.Folder.Folder>
    {
        public FolderDto()
        {
            Name = null!;
            Code = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public static FolderDto Create(Guid id, string name, string code)
        {
            return new FolderDto
            {
                Id = id,
                Name = name,
                Code = code
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.Folder.Folder, FolderDto>();
        }
    }
}