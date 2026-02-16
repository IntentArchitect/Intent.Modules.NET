using System;
using System.Collections.Generic;
using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.AdvancedMappingSystem.Clients.GetClientExtraFields
{
    public class GetClientExtraFieldsQuery : IRequest<List<ClientDto>>, IQuery
    {
        public GetClientExtraFieldsQuery(Guid id, string field1, string field2)
        {
            Id = id;
            Field1 = field1;
            Field2 = field2;
        }

        public Guid Id { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }
}