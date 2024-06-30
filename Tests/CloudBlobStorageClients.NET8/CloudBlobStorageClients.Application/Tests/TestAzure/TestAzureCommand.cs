using CloudBlobStorageClients.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CloudBlobStorageClients.Application.Tests.TestAzure
{
    public class TestAzureCommand : IRequest, ICommand
    {
        public TestAzureCommand()
        {
        }
    }
}