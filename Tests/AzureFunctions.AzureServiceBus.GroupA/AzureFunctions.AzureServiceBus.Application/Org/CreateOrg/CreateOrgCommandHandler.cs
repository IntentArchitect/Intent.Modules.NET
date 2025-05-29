using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.GroupA.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Application.Org.CreateOrg
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrgCommandHandler : IRequestHandler<CreateOrgCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateOrgCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateOrgCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Send(new CreateOrgIntegrationCommand
            {
                Name = request.Name,
                Type = request.Type,
                Founded = request.Founded,
                Departments = request.Departments
                    .Select(d => new OrgDepartmentDto
                    {
                        Name = d.Name,
                        Code = d.Code,
                        Description = d.Description
                    })
                    .ToList()
            });
        }
    }
}