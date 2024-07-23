using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Basics.CreateBasic
{
    public class CreateBasicCommand : IRequest<string>, ICommand
    {
        public CreateBasicCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}