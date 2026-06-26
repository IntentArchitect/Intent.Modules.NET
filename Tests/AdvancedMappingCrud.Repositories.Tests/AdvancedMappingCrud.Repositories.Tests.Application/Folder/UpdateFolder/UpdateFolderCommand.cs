using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Folder.UpdateFolder
{
    public class UpdateFolderCommand : IRequest, ICommand
    {
        public UpdateFolderCommand(Guid id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}