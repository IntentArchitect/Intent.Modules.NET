using Intent.RoslynWeaver.Attributes;
using MediatR;
using WebAndWorker.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace WebAndWorker.Application.App.Orders.UploadOrderDocument
{
    public class UploadOrderDocumentCommand : IRequest, ICommand
    {
        public UploadOrderDocumentCommand(Stream file, string fileName)
        {
            File = file;
            FileName = fileName;
        }

        public Stream File { get; set; }
        public string FileName { get; set; }
    }
}