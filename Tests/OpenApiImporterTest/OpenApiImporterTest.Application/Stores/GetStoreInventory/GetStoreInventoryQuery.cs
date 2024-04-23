using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.GetStoreInventory
{
    public class GetStoreInventoryQuery : IRequest<Dictionary<string, int>>, IQuery
    {
        public GetStoreInventoryQuery()
        {
        }
    }
}