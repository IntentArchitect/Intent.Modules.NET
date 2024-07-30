using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.CreateExplicitETag
{
    public class CreateExplicitETagCommand : IRequest<string>, ICommand
    {
        public CreateExplicitETagCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}