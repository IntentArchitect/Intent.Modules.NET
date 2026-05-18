using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Containers.CreateContainer
{
    public class CreateContainerCommand : IRequest<Guid>, ICommand
    {
        public CreateContainerCommand(string containerNumber, string sealNumber)
        {
            ContainerNumber = containerNumber;
            SealNumber = sealNumber;
        }

        public string ContainerNumber { get; set; }
        public string SealNumber { get; set; }
    }
}