using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.DeleteDocument
{
    public class DeleteDocumentCommand : IRequest, ICommand
    {
        public DeleteDocumentCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}