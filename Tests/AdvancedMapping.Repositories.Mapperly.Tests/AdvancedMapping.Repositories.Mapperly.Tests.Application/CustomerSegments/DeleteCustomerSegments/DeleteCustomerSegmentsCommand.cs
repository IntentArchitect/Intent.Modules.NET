using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.DeleteCustomerSegments
{
    public class DeleteCustomerSegmentsCommand : IRequest, ICommand
    {
        public DeleteCustomerSegmentsCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}