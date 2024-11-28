using System.Collections.Generic;
using AspNetCoreMvc.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AspNetCoreMvc.Application.ClientsApi.GetClients
{
    public class GetClientsQuery : IRequest<List<ClientDto>>, IQuery
    {
        public GetClientsQuery()
        {
        }
    }
}