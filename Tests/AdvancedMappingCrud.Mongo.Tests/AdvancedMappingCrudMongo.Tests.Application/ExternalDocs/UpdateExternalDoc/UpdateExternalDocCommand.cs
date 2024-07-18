using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.UpdateExternalDoc
{
    public class UpdateExternalDocCommand : IRequest, ICommand
    {
        public UpdateExternalDocCommand(string name, string thing, long id)
        {
            Name = name;
            Thing = thing;
            Id = id;
        }

        public string Name { get; set; }
        public string Thing { get; set; }
        public long Id { get; set; }
    }
}