using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.UpdateSimpleOdata
{
    public class UpdateSimpleOdataCommand : IRequest, ICommand
    {
        public UpdateSimpleOdataCommand(string name, string surname, string id)
        {
            Name = name;
            Surname = surname;
            Id = id;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Id { get; set; }
    }
}