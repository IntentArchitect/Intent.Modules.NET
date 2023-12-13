using System.Collections.Generic;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.GetNonStringPartitionKeys
{
    public class GetNonStringPartitionKeysQuery : IRequest<List<NonStringPartitionKeyDto>>, IQuery
    {
        public GetNonStringPartitionKeysQuery(int partInt)
        {
            PartInt = partInt;
        }

        public int PartInt { get; set; }
    }
}