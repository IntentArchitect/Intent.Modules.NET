using System.Collections.Generic;
using CosmosDB.EnumStrings.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.GetRootEntities
{
    public class GetRootEntitiesQuery : IRequest<List<RootEntityDto>>, IQuery
    {
        public GetRootEntitiesQuery()
        {
        }
    }
}