using System;
using System.IO;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.SimpleUpload
{
    public class SimpleUploadCommand : IRequest<Guid>, ICommand
    {
        public SimpleUploadCommand(Stream content)
        {
            Content = content;
        }

        public Stream Content { get; set; }
    }
}