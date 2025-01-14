using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.GetCNCCChildren
{
    public class GetCNCCChildrenQuery : IRequest<List<CNCCChildDto>>, IQuery
    {
        public GetCNCCChildrenQuery(Guid checkNewCompChildCrudId)
        {
            CheckNewCompChildCrudId = checkNewCompChildCrudId;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
    }
}