using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Tags.UpdateTag
{
    public class UpdateTagCommand : IRequest, ICommand
    {
        public UpdateTagCommand(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }
        public string Name { get; set; }

        public void SetId(string id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }
    }
}