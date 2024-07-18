using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.CreateExternalDoc
{
    public class CreateExternalDocCommand : IRequest<long>, ICommand
    {
        public CreateExternalDocCommand(long id, string name, string thing)
        {
            Id = id;
            Name = name;
            Thing = thing;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Thing { get; set; }
    }
}