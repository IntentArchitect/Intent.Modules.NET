using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Application.Common.Interfaces;
using ProxyServiceTests.Application.IntegrationServices.Contracts.OriginalServices.Services.Clients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ProxyServiceTests.Application.ClientsServices.FindClientById
{
    public class FindClientByIdQuery : IRequest<ClientDto>, IQuery
    {
        public FindClientByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}