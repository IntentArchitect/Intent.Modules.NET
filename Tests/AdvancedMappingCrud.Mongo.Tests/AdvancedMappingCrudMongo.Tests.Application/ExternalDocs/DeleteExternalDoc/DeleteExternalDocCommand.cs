using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.DeleteExternalDoc
{
    public class DeleteExternalDocCommand : IRequest, ICommand
    {
        public DeleteExternalDocCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}