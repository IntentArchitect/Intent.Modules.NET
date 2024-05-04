using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.CreateTag
{
    public class CreateTagCommand : IRequest<string>, ICommand
    {
        public CreateTagCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}