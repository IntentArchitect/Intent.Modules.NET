using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices;
using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallGetClientExtraFields
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallGetClientExtraFieldsCommandHandler : IRequestHandler<CallGetClientExtraFieldsCommand>
    {
        private readonly IClientsService _clientsService;

        [IntentManaged(Mode.Merge)]
        public CallGetClientExtraFieldsCommandHandler(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallGetClientExtraFieldsCommand request, CancellationToken cancellationToken)
        {
            var result = await _clientsService.GetClientExtraFieldsAsync(new GetClientExtraFieldsQuery
            {
                Id = request.Id,
                Field1 = request.Field1,
                Field2 = request.Field2
            }, cancellationToken);
        }
    }
}